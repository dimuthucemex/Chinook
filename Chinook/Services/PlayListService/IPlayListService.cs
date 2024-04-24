using Chinook.Models;

namespace Chinook.Services.PlayListService
{
    public interface IPlayListService
    {
        Task<IEnumerable<Playlist>> GetAllAsync();
        Task<Playlist> GetByIdAsync(long id);
        Task<Playlist> AddAsync(Playlist newArtist);
        Task<bool> UpdateAsync(Playlist artist);
        Task<bool> DeleteAsync(long id);
        Task<Playlist> GetByNameAsync(string name);
        Task<bool>ExistsAsync(string name, string currentUserId);
    }
}
