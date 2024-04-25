using Chinook.Models;

namespace Chinook.Services.Albums
{
    public interface IAlbumService
    {
        Task<IEnumerable<Album>> GetAlbumsForAtrtistAsync(int artistId);
    }
}
