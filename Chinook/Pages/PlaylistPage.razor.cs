using Chinook.Services.PlayListService;
using Chinook.Services.TrackService;
using Chinook.Services.UserPlayListService;
using Chinook.Utility;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using Chinook.Models;

namespace Chinook.Pages;
public partial class PlaylistPage : ComponentBase
{
    [Parameter] public long PlaylistId { get; set; }
    [CascadingParameter] private Task<AuthenticationState> authenticationState { get; set; }

    [Inject] IPlayListService PlayListService { get; set; }
    [Inject] ITrackService TrackService { get; set; }
    [Inject] IUserPlayListService UserPlaylistService { get; set; }

    private ClientModels.Playlist Playlist { get; set; }

    private string currentUserId = string.Empty;
    private string infoMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        currentUserId = await GetUserId();

        await InvokeAsync(StateHasChanged);
        Playlist = await PlayListService.GetByIdAsync(PlaylistId, currentUserId);
        this.PlaylistId = Playlist.PlaylistId;
    }

    protected override async Task OnParametersSetAsync()
    {
        Console.WriteLine("Parameter changed");
        if (this.PlaylistId != Playlist.PlaylistId)
        {
            await Task.Delay(100);
            Playlist = await PlayListService.GetByIdAsync(this.PlaylistId, currentUserId);

        }
    }

    private async Task<string> GetUserId()
    {
        var user = (await authenticationState).User;
        var userId = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;
        return userId;
    }

    private async Task FavoriteTrack(long trackId)
    {
        var track = Playlist.Tracks.SingleOrDefault(t => t.TrackId == trackId);

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

    private async Task UnfavoriteTrack(long trackId)
    {
        var track = Playlist.Tracks.FirstOrDefault(t => t.TrackId == trackId);

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

    /// <summary>
    /// Remove Track from Playlist
    /// </summary>
    /// <param name="trackId"></param>
    /// <returns></returns>
    private async Task RemoveTrack(long trackId)
    {
        var playListId = Playlist.PlaylistId;
        var track = Playlist.Tracks.FirstOrDefault(f => f.TrackId == trackId);
        await TrackService.RemoveFromPlayListAsync(trackId, playListId);

        infoMessage = $"Track {track?.ArtistName} - {track?.AlbumTitle} - {track?.TrackName} removed from playlist {AppCommons.Favorites}.";

        Playlist = await PlayListService.GetByIdAsync(PlaylistId, currentUserId);
    }

    private void CloseInfoMessage()
    {
        infoMessage = "";
    }
}
