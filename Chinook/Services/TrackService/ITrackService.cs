using Chinook.ClientModels;
using Chinook.Models;

namespace Chinook.Services.TrackService
{
    public interface ITrackService
    {
        Task<IEnumerable<Track>> GetAllAsync();
        Task<Track> GetByIdAsync(long id);
        Task<Track> AddAsync(Track newArtist);
        Task<bool> UpdateAsync(Track artist);
        Task<bool> DeleteAsync(long id);
        Task<IEnumerable<PlaylistTrack>> GetByArtistIdAsync(long aritstId , string currentUserId);
        Task<bool> AddToPlayList(long trackId, long playListId);
        Task<bool> RemoveFromPlayListAsync(long trackId, long playListId);

    }
}
