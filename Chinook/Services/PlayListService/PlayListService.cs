using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services.PlayListService
{
    public class PlayListService : DbContext , IPlayListService
    {
        public async Task<Playlist> AddAsync(Playlist newPlayList)
        {
            using var context = new ChinookContext();
            await context.Playlists.AddAsync(newPlayList);
            await context.SaveChangesAsync();
            return newPlayList;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var context = new ChinookContext();
            var playList = await context.Playlists.FindAsync(id);
            if (playList == null)
            {
                return false;
            }
            context.Playlists.Remove(playList);
            return await context.SaveChangesAsync()>0;
        }

        public async Task<IEnumerable<Models.Playlist>> GetAllAsync()
        {
            using var context = new ChinookContext();
            return await context.Playlists.Include(p => p.Tracks).ToListAsync();
        }

        public async Task<Playlist> GetByIdAsync(long id)
        {
            using var context = new ChinookContext();
            return await context.Playlists.FindAsync(id);
        }

        public async Task<Playlist> GetByNameAsync(string name)
        {
            using var context = new ChinookContext();
            return await context.Playlists.FirstOrDefaultAsync(a => a.Name == name);
        }

        public async Task<bool> UpdateAsync(Playlist playList)
        {
            using var context = new ChinookContext();
            context.Entry(playList).State = EntityState.Modified;
            return await context.SaveChangesAsync() > 0;
        }

    }

}
