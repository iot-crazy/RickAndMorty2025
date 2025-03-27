using Microsoft.AspNetCore.Components;
using MudBlazor;
using RickAndMorty.Contracts;
using RickAndMorty.DTO.Character;
using RickAndMorty.DTO.Episode;
using RickAndMorty.DTO.Location;

namespace RickAndMorty.Web.Client.Pages;

public class CharactersBase : ComponentBase
{

    [Inject]
    private ICharacterDataProvider CharacterProvider { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; }

    protected List<CharacterDto> AllCharacters = [];
    protected NewCharacterDto NewCharacter = new();
    protected MudDialog AddNewCharacterDialog { get; set; } = default!;
    protected MudForm AddNewCharacterForm { get; set; } = default!;
    protected DialogOptions DialogOptions = default!;
    protected MudDialog EpisodesDialog { get; set; } = default!;
    protected DialogOptions EpisodeDialogOptions = default!;

    protected int SelectedCharacterId { get; set; } = 0;
    protected string SelectedCharacterName { get => AllCharacters.First(x => x.Id == SelectedCharacterId).Name; }
    protected List<EpisodeDto> EpisodesForSelectedCharacter { get => AllCharacters.First(x => x.Id == SelectedCharacterId).Episodes; }

    protected string SearchString { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        DialogOptions = new()
        {
            FullWidth = true,
            MaxWidth = MaxWidth.ExtraLarge,
            Position = DialogPosition.Center,
            BackdropClick = false
        };

        EpisodeDialogOptions = new()
        {
            FullWidth = false,
            MaxWidth = MaxWidth.Large,
            Position = DialogPosition.Center,
            BackdropClick = true,
            CloseButton = true,
            CloseOnEscapeKey = true,

        };

        await GetAllCharacters();
        StateHasChanged();
    }

    protected async Task AddNewDialogOpen()
    {
        NewCharacter = new();
        await AddNewCharacterDialog.ShowAsync();
        await InvokeAsync(StateHasChanged);
    }

    protected async Task AddnewDialogClose()
    {
        await AddNewCharacterDialog.CloseAsync();
        await InvokeAsync(StateHasChanged);
    }

    protected void OnSelectedLocationChanged(LocationDto location)
    {
        NewCharacter.LocationId = location.Id;
    }

    protected void OnSelectedOriginChanged(LocationDto origin)
    {
        NewCharacter.OriginLocationId = origin.Id;
    }

    protected void OnEpisodesChanged(List<EpisodeDto> episodes)
    {
        NewCharacter.Episodes = episodes.Select(x => x.Id).ToList();
    }

    protected async Task SubmitNewCharacter()
    {
        await AddNewCharacterForm.Validate();
        if (AddNewCharacterForm.IsValid)
        {
            await CharacterProvider.AddAsync(NewCharacter);
            await GetAllCharacters();
            await AddNewCharacterDialog.CloseAsync();
            Snackbar.Add($"New character added {NewCharacter.Name}");
            await InvokeAsync(StateHasChanged);
        }
    }

    protected async Task OpenEpisodes(int characterId)
    {
        SelectedCharacterId = characterId;
        await EpisodesDialog.ShowAsync();
        await InvokeAsync(StateHasChanged);
    }

    protected bool FilterFunc(CharacterDto character) => Filter(character, SearchString);

    private bool Filter(CharacterDto character, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;

        if (character.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if (character.Status.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if (character.Location != null &&
            (character.Location.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
            || character.Location.Dimension.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            )
            return true;

        if (character.Origin != null &&
            (character.Origin.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
            || character.Origin.Dimension.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            )
            return true;

        if (character.Location != null && character.Location.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    private async Task GetAllCharacters()
    {
        AllCharacters = (await CharacterProvider.GetAsync()).ToList();
    }
}