using Microsoft.AspNetCore.Components;
using RickAndMorty.Contracts;
using RickAndMorty.DTO.Character;

namespace RickAndMorty.Web.Client.Pages;

public class HomeBase : ComponentBase
{
    [Inject]
    private ICharacterDataProvider CharacterProvider { get; set; } = default!;

    [Inject]
    private IImportService ImportService { get; set; } = default!;

    protected List<CharacterDto> AllCharacters = [];
    protected bool ImportInProgress = false;


    protected override async Task OnInitializedAsync()
    {
        await GetAllCharacters();
        StateHasChanged();
    }

    protected async Task ImportData()
    {
        ImportInProgress = true;
        await InvokeAsync(StateHasChanged);
        await ImportService.StartAsync();
        await GetAllCharacters();
        ImportInProgress = true;
        await InvokeAsync(StateHasChanged);
    }

    private async Task GetAllCharacters()
    {
        AllCharacters = (await CharacterProvider.GetAsync()).ToList();
    }
}
