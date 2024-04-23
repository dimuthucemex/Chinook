using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services.UserPlayListService
{
    public class UserPlayListService : DbContext, IUserPlayListService
    {
        public async Task<UserPlaylist> AddAsync(UserPlaylist userPlayList)
        {
            using var context = new ChinookContext();
            await context.UserPlaylists.AddAsync(userPlayList);
            await context.SaveChangesAsync();
            return userPlayList;
        }

        //public Task<UserPlaylist> AddToFavouriteAsync(long trackId, string currentUserId)
        //{
        //    using var context = new ChinookContext();
        //    var favouritePlaylist = context.Playlists.FirstOrDefault(p => p.Name == "Favorites");
        //    if (favouritePlaylist == null)
        //    {
        //        favouritePlaylist = new Playlist()
        //        {
        //            Name = "Favorites"
        //        };
        //        context.Playlists.Add(favouritePlaylist);
        //        context.SaveChanges();
        //    }
        //    var userPlaylist = new UserPlaylist()
        //    {
        //        PlaylistId = favouritePlaylist.PlaylistId,
        //        TrackId = trackId,
        //        UserId = currentUserId
        //    };
        //}

        //public async Task<bool> DeleteAsync(long id)
        //{
        //    using var context = new ChinookContext();
        //    var track = await context.UserPlaylists.FirstOrDefaultAsync(a => a.use == id);
        //    if (track == null)
        //    {
        //        return false;
        //    }
        //    context.UserPlaylists.Remove(track);
        //    return await context.SaveChangesAsync() > 0;
        //}

        //public async Task<IEnumerable<Track>> GetAllAsync()
        //{
        //    using var context = new ChinookContext();
        //    return await context.Tracks.ToListAsync();
        //}

        //public async Task<IEnumerable<PlaylistTrack>> GetByArtistIdAsync(long aritstId, string currentUserId)
        //{
        //    using var context = new ChinookContext();
        //    return await context.Tracks.Where(a => a.Album.ArtistId == aritstId)
        //    .Include(a => a.Album)
        //    .Select(t => new PlaylistTrack()
        //    {
        //        AlbumTitle = t.Album == null ? "-" : t.Album.Title,
        //        TrackId = t.TrackId,
        //        TrackName = t.Name,
        //        IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == currentUserId && up.Playlist.Name == "Favorites")).Any()
        //    })
        //    .ToListAsync();
        //}

        //public async Task<Track> GetByIdAsync(long id)
        //{
        //    using var context = new ChinookContext();
        //    return await context.Tracks.FindAsync(id);

        //}

        //public async Task<bool> UpdateAsync(Track artist)
        //{
        //    using var context = new ChinookContext();
        //    context.Entry(artist).State = EntityState.Modified;
        //    return await context.SaveChangesAsync() > 0;
        //}

    }
}
