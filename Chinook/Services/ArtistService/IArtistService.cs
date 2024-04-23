using Chinook.Models;
namespace Chinook.Services.ArtistService
{
    public interface IArtistService
    {
        Task<IEnumerable<Artist>> GetAllAsync();
        Task<Artist> GetByIdAsync(long id);
        Task<Artist> AddAsync(Artist newArtist);
        Task<bool> UpdateAsync(Artist artist);
        Task<bool> DeleteAsync(long id);

    }
}
