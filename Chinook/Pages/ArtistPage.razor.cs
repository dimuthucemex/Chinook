using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Services.ArtistService;
using Chinook.Services.PlayListService;
using Chinook.Services.TrackService;
using Chinook.Services.UserPlayListService;
using Chinook.Shared.Components;
using Chinook.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Chinook.Pages;
public partial class ArtistPage : ComponentBase
{
    [Parameter] public long ArtistId { get; set; }
    [CascadingParameter] private Task<AuthenticationState> authenticationState { get; set; }

    [Inject] IArtistService ArtistService { get; set; }
    [Inject] ITrackService TrackService { get; set; }
    [Inject] IUserPlayListService UserPlaylistService { get; set; }
    [Inject] IPlayListService PlayListService { get; set; }


    private Modal PlaylistDialog { get; set; }
    private Artist Artist { get; set; }
    private PlaylistTrack? SelectedTrack { get; set; }
    private IEnumerable<PlaylistTrack> Tracks { get; set; }
    private IEnumerable<UserPlayList> UserPlayLists { get; set; }

    private string infoMessage;
    private string currentUserId;
    private long playListId;
    private string newPlayListName;


    protected override async Task OnInitializedAsync()
    {
        await InvokeAsync(StateHasChanged);

        currentUserId = await GetUserId();
        Artist = await ArtistService.GetByIdAsync(ArtistId);
        Tracks = await TrackService.GetByArtistIdAsync(ArtistId, currentUserId);
        UserPlayLists = await UserPlaylistService.GetAllAsync(currentUserId);

    }

    /// <summary>
    /// Get the logged in User's Id from the ClaimsPrincipal. The Id is stored in the ClaimType 'NameIdentifier'
    /// </summary>
    /// <returns></returns> 
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
    private async Task FavoriteTrack(long trackId)
    {
        var track = Tracks.SingleOrDefault(t => t.TrackId == trackId);

        // If Track is not null then add to Favorites playlist
        if (track != null)
        {
            track.IsFavorite = true;
            await AddToFavouriteAsync(trackId, currentUserId);
            infoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} added to playlist {AppCommons.Favorites}.";
        }
        else
        {
            infoMessage = $"Track cannot be found.";
        }
    }

    /// <summary>
    /// Remove track from the Favorites playlist
    /// </summary>
    /// <param name="trackId"></param>
    private async void UnfavoriteTrack(long trackId)
    {
        var track = Tracks.FirstOrDefault(t => t.TrackId == trackId);

        // if track is not null, then remove from Favorites playlist
        if (track != null)
        {
            track.IsFavorite = false;
            await RemoveFromFavorites(trackId, currentUserId);
            infoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} removed from playlist {AppCommons.Favorites}.";
        }
        else
        {
            infoMessage = $"Track cannot be found.";
        }
    }

    /// <summary>
    /// On dropdown value change event, update the selected value
    /// </summary>
    /// <param name="e"></param>
    void HandleSelection(ChangeEventArgs e)
    {
        playListId = e.Value != null ? Convert.ToInt64(e.Value) : 0;
    }

    /// <summary>
    /// Open the playlist dialog
    /// </summary>
    /// <param name="trackId"></param>
    private void OpenPlaylistDialog(long trackId)
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
            if (playListId == 0 && !string.IsNullOrEmpty(newPlayListName))
            {
                // Add New Playlist
                var alreadyExists = await PlayListService.ExistsAsync(newPlayListName, currentUserId);
                if (!alreadyExists)
                {
                    var addedPlayList = await PlayListService.AddAsync(new Models.Playlist { Name = newPlayListName });
                    var userPlayList = await UserPlaylistService.AddAsync(new UserPlaylist { PlaylistId = addedPlayList.PlaylistId, UserId = currentUserId });
                    await TrackService.AddToPlayList(SelectedTrack.TrackId, addedPlayList.PlaylistId);
                    CloseInfoMessage();
                    infoMessage = $"Track {Artist.Name} - {SelectedTrack.AlbumTitle} - {SelectedTrack.TrackName} added to playlist {newPlayListName}.";
                }
                else
                {
                    infoMessage = $"{newPlayListName} already exists";
                }
            }
            else
            {
                // Add to Existing Playlist
                await TrackService.AddToPlayList(SelectedTrack.TrackId, playListId);
                var userPlayList = UserPlayLists.FirstOrDefault(f => f.PlaylistId == playListId);
                var playList = userPlayList?.Playlist;
                CloseInfoMessage();
                infoMessage = $"Track {Artist.Name} - {SelectedTrack.AlbumTitle} - {SelectedTrack.TrackName} added to playlist {playList.Name}.";
            }
            PlaylistDialog.Close();

        }
        catch (Exception e)
        {
            PlaylistDialog.Close();
            infoMessage = $"Operation failed. Please try again.";
        }
    }

    /// <summary>
    /// Close the info message
    /// </summary>
    private void CloseInfoMessage()
    {
        infoMessage = "";
    }

    /// <summary>
    /// Add to Favorites Playlist
    /// </summary>
    /// <param name="trackId"></param>
    /// <param name="currentUserId"></param>
    /// <returns></returns>
    private async Task AddToFavouriteAsync(long trackId, string currentUserId)
    {
        try
        {
            var favouritePlayList = await PlayListService.GetByNameAsync(AppCommons.Favorites);
            if (favouritePlayList == null)
            {
                favouritePlayList = await PlayListService.AddAsync(new Models.Playlist
                {
                    Name = AppCommons.Favorites
                }
                );
            }
            var userPlayList = new UserPlaylist { PlaylistId = favouritePlayList.PlaylistId, UserId = currentUserId };
            await TrackService.AddToPlayList(trackId, userPlayList.PlaylistId);
            await UserPlaylistService.AddAsync(userPlayList);
        }
        catch (Exception e)
        {
            infoMessage = $"Operation failed. Please try again.";
        }
    }

    /// <summary>
    /// Remove from Favorites Playlist
    /// </summary>
    /// <param name="trackId"></param>
    /// <param name="currentUserId"></param>
    /// <returns></returns>
    private async Task RemoveFromFavorites(long trackId, string currentUserId)
    {
        var favoritePlayList = await PlayListService.GetByNameAsync(AppCommons.Favorites);
        if (favoritePlayList != null)
        {
            await UserPlaylistService.DeleteAsync(favoritePlayList.PlaylistId, currentUserId);
        }
    }

}
