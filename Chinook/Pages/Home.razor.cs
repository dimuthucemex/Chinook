using Chinook.Models;
using Chinook.Services.Albums;
using Chinook.Services.ArtistService;
using Microsoft.AspNetCore.Components;

namespace Chinook.Pages;
public partial class Home : ComponentBase
{

    [Inject] IArtistService ArtistService { get; set; }
    [Inject] IAlbumService AlbumService { get; set; }

    private IEnumerable<Artist> Artists = new List<Artist>();
    private string searchQuery = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await InvokeAsync(StateHasChanged);
        Artists = await ArtistService.GetAllAsync();
    }

    public async Task<IEnumerable<Artist>> GetArtists() =>
        await ArtistService.GetAllAsync();

    public async Task<IEnumerable<Album>> GetAlbumsForArtist(int artistId) =>
        await AlbumService.GetAlbumsForAtrtistAsync(artistId);

    private async Task Search()
    {
        if (string.IsNullOrEmpty(searchQuery))
        {
            Artists = await GetArtists();
        }
        else
        {
            Artists = Artists.Where(a => a.Name != null && a.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}
