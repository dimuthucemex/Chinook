namespace Chinook.Services.PlayListService
{
    public interface IPlayListService
    {
        Task<ClientModels.Playlist> GetByIdAsync(long id, string currentUserId);
        Task<Models.Playlist> AddAsync(Models.Playlist newArtist);
        Task<bool> UpdateAsync(Models.Playlist artist);
        Task<bool> DeleteAsync(long id);
        Task<Models.Playlist> GetByNameAsync(string name);
        Task<bool> ExistsAsync(string name, string currentUserId);
    }
}
