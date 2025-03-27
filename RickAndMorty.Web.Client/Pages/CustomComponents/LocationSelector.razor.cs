using Microsoft.AspNetCore.Components;
using RickAndMorty.Contracts;
using RickAndMorty.DTO.Location;

namespace RickAndMorty.Web.Client.Pages.CustomComponents;

public class LocationSelectorBase : ComponentBase
{
    private LocationDto _selectedLocation = null!;

    [Inject]
    protected ILocationDataProvider LocationDataProvider { get; set; } = default!;

    [Parameter]
    public EventCallback<LocationDto> SelectedLocationChanged { get; set; }

    [Parameter]
    public string Placeholder { get; set; } = string.Empty;

    [Parameter]
    public string Label { get; set; } = string.Empty;

    [Parameter]
    public LocationDto SelectedLocation
    {
        get => _selectedLocation;

        set
        {
            if (_selectedLocation == value) return;
            _selectedLocation = value;
            SelectedLocationChanged.InvokeAsync(value);
        }
    }

    protected IEnumerable<LocationDto> Locations = [];

    protected async Task OnSelectedLocationChanged(LocationDto location)
    {
        SelectedLocation = location;
        await SelectedLocationChanged.InvokeAsync(location);
    }

    protected override async Task OnInitializedAsync()
    {
        await GetLocations();
        SelectedLocation = Locations.First(x => x.Id == 0);
        StateHasChanged();
    }

    private async Task GetLocations()
    {
        Locations = (await LocationDataProvider.GetAsync()).OrderBy(x => x.Name);
    }

}