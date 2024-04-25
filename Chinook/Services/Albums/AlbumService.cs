using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services.Albums
{
    public class AlbumService : IAlbumService
    {
        public async Task<IEnumerable<Album>> GetAlbumsForAtrtistAsync(int artistId)
        {
            using var context = new ChinookContext();
            return await context.Albums.Where(w=>w.AlbumId == artistId).ToListAsync();
        }
    }
}
