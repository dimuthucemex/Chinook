using Chinook.ClientModels;
using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services.UserPlayListService
{
    public class UserPlayListService : DbContext, IUserPlayListService
    {
        public async Task<UserPlaylist> AddAsync(UserPlaylist userPlayList)
        {
            using var context = new ChinookContext();
            await context.UserPlaylists.AddAsync(userPlayList);

            await context.SaveChangesAsync();
            return userPlayList;
        }

        public async Task<bool> DeleteAsync(long playListId, string currentUserId)
        {
            using var context = new ChinookContext();
            var playList = await context.Playlists.FirstOrDefaultAsync(a => a.PlaylistId == playListId);
            if (playList == null)
            {
                return false;
            }
            var userPlayList = await context.UserPlaylists.FirstOrDefaultAsync(a => a.PlaylistId == playListId && a.UserId == currentUserId);
            if(userPlayList == null)
            {
                return false;
            }

            context.UserPlaylists.Remove(userPlayList);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<UserPlayList>> GetAllAsync(string currentUserId)
        {
            using var context = new ChinookContext();
            return await context.UserPlaylists
                .Include(i => i.Playlist)
                .Where(w => w.UserId == currentUserId)
                .Select(s => new UserPlayList
                {
                    PlaylistId = s.PlaylistId,
                    UserId = s.UserId,
                    Playlist = s.Playlist
                }).ToListAsync();
        }
    }
}
