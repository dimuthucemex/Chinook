using Chinook.ClientModels;
using Chinook.Migrations;
using Chinook.Models;
using Chinook.Services.PlayListService;
using Chinook.Services.TrackService;
using Chinook.Services.UserPlayListService;
using NuGet.DependencyResolver;

namespace Chinook.Utility
{
    public class TrackServiceUtility
    {
        private readonly IPlayListService PlayListService;
        private readonly IUserPlayListService UserPlayListService;

        public TrackServiceUtility(IPlayListService playListService, IUserPlayListService userPlayListService)
        {
            PlayListService = playListService;
            UserPlayListService = userPlayListService;
        }

        public static async Task<string> AddTrackToPlaylistAsync(long playListId, PlaylistTrack selectedTrack, string currentUserId, string newPlayListName)
        {
            var favouritePlayList = await PlayListService.GetByNameAsync(AppCommons.Favorites);
            if (favouritePlayList == null)
            {
                favouritePlayList = await PlayListService.AddAsync(new Models.Playlist
                {
                    Name = AppCommons.Favorites
                });
            }
            var userPlayList = new UserPlaylist { PlaylistId = favouritePlayList.PlaylistId, UserId = currentUserId };
            await TrackService.AddToPlayList(selectedTrack.TrackId, userPlayList.PlaylistId);
            await UserPlayListService.AddAsync(userPlayList);

            return ""; // Add a return statement here
        }
    }
}
