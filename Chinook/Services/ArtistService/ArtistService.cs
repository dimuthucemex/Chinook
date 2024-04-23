using Microsoft.EntityFrameworkCore;

namespace Chinook.Services.ArtistService
{
    public class ArtistService : DbContext, IArtistService
    {
        public async Task<Models.Artist> AddAsync(Models.Artist newArtist)
        {
            using var context = new ChinookContext();
            await context.Artists.FindAsync(newArtist);
            await context.SaveChangesAsync();
            return newArtist;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var context = new ChinookContext();
            var artist = await context.Artists.FindAsync(id);
            if (artist == null)
            {
                return false;
            }
            context.Artists.Remove(artist);
            return await context.SaveChangesAsync()>0;
        }

        public async Task<IEnumerable<Models.Artist>> GetAllAsync()
        {
            using var context = new ChinookContext();
            return await context.Artists.Include(a => a.Albums).ToListAsync();
        }

        public async Task<Models.Artist> GetByIdAsync(long id)
        {
            using var context = new ChinookContext();
            return await context.Artists.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(Models.Artist artist)
        {
            using var context = new ChinookContext();
            context.Entry(artist).State = EntityState.Modified;
            return await context.SaveChangesAsync() > 0;
        }
    }
}
