using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Utility;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services.TrackService
{
    public class TrackService : DbContext, ITrackService
    {
        public async Task<Track> AddAsync(Track newArtist)
        {
            using var context = new ChinookContext();   
            await context.Tracks.AddAsync(newArtist);
            await context.SaveChangesAsync();
            return newArtist;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var context = new ChinookContext();
            var track = await context.Tracks.FirstOrDefaultAsync(a => a.TrackId == id);
            if(track == null)
            {
                return false;
            }   
            context.Tracks.Remove(track);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Track>> GetAllAsync()
        {
            using var context = new ChinookContext();
            return await context.Tracks.ToListAsync();
        }

        public async Task<IEnumerable<PlaylistTrack>> GetByArtistIdAsync(long aritstId,string currentUserId)
        {
            using var context = new ChinookContext();
            var result = await context.Tracks.Where(a => a.Album.ArtistId == aritstId)
            .Include(a => a.Album)
            .Include(a => a.Playlists)
            .ThenInclude(a => a.UserPlaylists)
            .ToListAsync();


            return result.Select(t => new PlaylistTrack()
            {
                AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                TrackId = t.TrackId,
                TrackName = t.Name,
                IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == currentUserId && up.Playlist.Name == AppCommons.Favorites)).Any()
            }).ToList();
        }

        public async Task<Track> GetByIdAsync(long id)
        {
            using var context = new ChinookContext();
            return await context.Tracks.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(Track artist)
        {
            using var context = new ChinookContext();
            context.Entry(artist).State = EntityState.Modified;
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddToPlayList(long trackId, long playListId)
        {
            using var context = new ChinookContext();
            var track = await context.Tracks.Include(i=>i.Playlists).FirstOrDefaultAsync(f=>f.TrackId == trackId);
            var playList = await context.Playlists.Include(i=>i.UserPlaylists).FirstOrDefaultAsync(f=>f.PlaylistId == playListId);
            
            if(track == null && playList == null)
            {
                return false;
            }
            else
            {
                if(track.Playlists.Any(a=>a.PlaylistId == playListId))
                {
                    return false;
                }
                track.Playlists.Add(playList);
                return await context.SaveChangesAsync() > 0;
            }

        }

        public async Task<bool> RemoveFromPlayListAsync(long trackId, long playListId)
        {
            using var context = new ChinookContext();
            var track = await context.Tracks.Include(i=>i.Playlists).FirstOrDefaultAsync(f=>f.TrackId == trackId);
            var playList = await context.Playlists.Include(i=>i.Tracks).FirstOrDefaultAsync(f=>f.PlaylistId == playListId);
            
            if(track == null && playList == null)
            {
                return false;
            }
            else
            {
                if(!track.Playlists.Any(a=>a.PlaylistId == playListId))
                {
                    return false;
                }
                playList.Tracks.Remove(track);
                return await context.SaveChangesAsync() > 0;
            }
        }

    }
}
