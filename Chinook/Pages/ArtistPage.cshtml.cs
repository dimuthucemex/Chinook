using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Services.ArtistService;
using Chinook.Services.PlayListService;
using Chinook.Services.TrackService;
using Chinook.Services.UserPlayListService;
using Chinook.Shared.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Chinook.Pages;
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[Microsoft.AspNetCore.Components.Route("/artist/{ArtistId}")]
[Authorize]
public partial class ArtistPageModel : ComponentBase
{
    public long ArtistId { get; set; }
    [CascadingParameter] private Task<AuthenticationState> authenticationState { get; set; }

    private readonly IArtistService ArtistService;
    private readonly ITrackService TrackService;
    private readonly IUserPlayListService UserPlaylistService;
    private readonly IPlayListService PlayListService;

    private Modal PlaylistDialog { get; set; }
    public Artist Artist { get; set; }
    public PlaylistTrack SelectedTrack { get; set; }
    public IEnumerable<PlaylistTrack> Tracks { get;set; }
    public IEnumerable<UserPlayList> UserPlayLists { get; set;}
    public string InfoMessage { get; set; }
    public string CurrentUserId { get; set; }
    public long PlayListId { get; set; }
    public string NewPlayListName;

    // Define an event that will be triggered when a playlist is added or modified
    public event EventHandler PlaylistsUpdated;

    public ArtistPageModel()
    {
    }   

    public ArtistPageModel(IArtistService artistService, ITrackService trackService, 
        IUserPlayListService userPlaylistService, IPlayListService playListService)
    {
        ArtistService = artistService;
        TrackService = trackService;
        UserPlaylistService = userPlaylistService;
        PlayListService = playListService;
    }

    protected override async Task OnInitializedAsync()
    {
        StateHasChanged();
        CurrentUserId = await GetUserId();

        Artist = await ArtistService.GetByIdAsync(ArtistId);

        Tracks = await TrackService.GetByArtistIdAsync(ArtistId, CurrentUserId);

        UserPlayLists = await UserPlaylistService.GetAllAsync(CurrentUserId);
    }

    private async Task<string> GetUserId()
    {
        var user = (await authenticationState).User;
        var userId = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;
        return userId;
    }

    /// <summary>
    /// Add track to the Favorites playlist
    /// </summary>
    /// <param name="trackId"></param>
    /// <returns></returns>
    public async Task FavoriteTrack(long trackId)
    {
        var track = Tracks.SingleOrDefault(t => t.TrackId == trackId);

        // If Track is not null then add to Favorites playlist
        if (track != null)
        {
            track.IsFavorite = true;
            await AddToFavouriteAsync(trackId, CurrentUserId);
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} added to playlist Favorites.";
        }
        else
        {
            InfoMessage = $"Track cannot be found.";
        }
    }

    /// <summary>
    /// Remove track from the Favorites playlist
    /// </summary>
    /// <param name="trackId"></param>
    public async void UnfavoriteTrack(long trackId)
    {
        var track = Tracks.FirstOrDefault(t => t.TrackId == trackId);

        // if track is not null, then remove from Favorites playlist
        if (track != null)
        {
            track.IsFavorite = false;
            await RemoveFromFavorites(trackId, CurrentUserId);
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} removed from playlist Favorites.";
        }
        else
        {
            InfoMessage = $"Track cannot be found.";
        }
    }

    /// <summary>
    /// On dropdown value change event, update the selected value
    /// </summary>
    /// <param name="e"></param>
    public void HandleSelection(ChangeEventArgs e)
    {
        PlayListId = e.Value != null ? Convert.ToInt64(e.Value) : 0;
    }

    /// <summary>
    /// Open the playlist dialog
    /// </summary>
    /// <param name="trackId"></param>
    public void OpenPlaylistDialog(long trackId)
    {
        CloseInfoMessage();
        SelectedTrack = Tracks.FirstOrDefault(t => t.TrackId == trackId);
        PlaylistDialog.Open();
    }

    /// <summary>
    /// Add Track to Playlist
    /// </summary>
    /// <returns></returns>
    private async Task AddTrackToPlaylist()
    {
        try
        {
            if (PlayListId == 0 && !string.IsNullOrEmpty(NewPlayListName))
            {
                // Add New Playlist
                var alreadyExists = await PlayListService.ExistsAsync(NewPlayListName, CurrentUserId);
                if (!alreadyExists)
                {
                    var addedPlayList = await PlayListService.AddAsync(new Models.Playlist { Name = NewPlayListName });
                    var userPlayList = await UserPlaylistService.AddAsync(new UserPlaylist { PlaylistId = addedPlayList.PlaylistId, UserId = CurrentUserId });
                    OnPlaylistsUpdated();

                    CloseInfoMessage();
                    InfoMessage = $"Track {Artist.Name} - {SelectedTrack.AlbumTitle} - {SelectedTrack.TrackName} added to playlist {NewPlayListName}.";
                    StateHasChanged();
                }
                else
                {
                    InfoMessage = $"{NewPlayListName} already exists";
                }
            }
            else
            {
                // Add to Existing Playlist
                await TrackService.AddToPlayList(SelectedTrack.TrackId, PlayListId);
                var userPlayList = UserPlayLists.FirstOrDefault(f => f.PlaylistId == PlayListId);
                var playList = userPlayList?.Playlist;
                CloseInfoMessage();
                InfoMessage = $"Track {Artist.Name} - {SelectedTrack.AlbumTitle} - {SelectedTrack.TrackName} added to playlist {playList.Name}.";
            }
            PlaylistDialog.Close();
        }
        catch (Exception e)
        {
            PlaylistDialog.Close();
            InfoMessage = $"Operation failed. Please try again.";
        }
    }

    // Method to trigger the event
    protected virtual void OnPlaylistsUpdated()
    {
        PlaylistsUpdated?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Close the info message
    /// </summary>
    private void CloseInfoMessage()
    {
        InfoMessage = "";
    }

    private async Task AddToFavouriteAsync(long trackId, string currentUserId)
    {
        try
        {
            var favouritePlayList = await PlayListService.GetByNameAsync("Favorites");
            if (favouritePlayList == null)
            {
                favouritePlayList = await PlayListService.AddAsync(new Models.Playlist
                {
                    Name = "Favorites"
                });
            }
            var userPlayList = new UserPlaylist { PlaylistId = favouritePlayList.PlaylistId, UserId = currentUserId };
            await TrackService.AddToPlayList(trackId, userPlayList.PlaylistId);
            await UserPlaylistService.AddAsync(userPlayList);
        }
        catch (Exception e)
        {
            InfoMessage = $"Operation failed. Please try again.";
        }
    }

    private async Task RemoveFromFavorites(long trackId, string currentUserId)
    {
        var favoritePlayList = await PlayListService.GetByNameAsync("Favorites");
        if (favoritePlayList != null)
        {
            await UserPlaylistService.DeleteAsync(favoritePlayList.PlaylistId, currentUserId);
        }
    }
}
