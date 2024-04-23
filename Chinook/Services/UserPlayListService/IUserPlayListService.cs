using Chinook.Models;

namespace Chinook.Services.UserPlayListService
{
    public interface IUserPlayListService
    {
        Task<UserPlaylist> AddAsync(UserPlaylist newUserPlaylist);
        //Task<UserPlaylist> AddToFavouriteAsync(long trackId, string currentUserId);
        //Task<IEnumerable<UserPlaylist>> GetAllAsync();
        //Task<UserPlaylist> GetByIdAsync(long id);
        //Task<bool> UpdateAsync(UserPlaylist artist);
        //Task<bool> DeleteAsync(long id);

    }
}
