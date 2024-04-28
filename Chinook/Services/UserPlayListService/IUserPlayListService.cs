using Chinook.ClientModels;
using Chinook.Models;

namespace Chinook.Services.UserPlayListService
{
    public interface IUserPlayListService
    {
        Task<UserPlaylist> AddAsync(UserPlaylist newUserPlaylist);
        Task<bool> DeleteAsync(long trackId, string currentUserId);
        Task<IEnumerable<ClientModels.UserPlayList>> GetAllAsync(string currentUserId);
        void OnPlaylistsUpdated();
    }
}
