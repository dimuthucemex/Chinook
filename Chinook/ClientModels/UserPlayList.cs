using Chinook.Models;

namespace Chinook.ClientModels
{
    public class UserPlayList
    {
        public string UserId { get; set; }
        public long PlaylistId { get; set; }
        public ChinookUser User { get; set; }
        public Models.Playlist Playlist { get; set; }
    }
}
