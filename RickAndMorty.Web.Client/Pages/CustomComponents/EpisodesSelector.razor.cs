using Microsoft.AspNetCore.Components;
using RickAndMorty.Contracts;
using RickAndMorty.DTO.Episode;

namespace RickAndMorty.Web.Client.Pages.CustomComponents;

public class EpisodesSelectorBase : ComponentBase
{

    protected IEnumerable<EpisodeDto> Episodes = [];
    private List<EpisodeDto> _selectedEpisodes = null!;

    [Inject]
    protected IEpisodeDataProvider EpisodeDataProvider { get; set; } = default!;

    [Parameter]
    public EventCallback<List<EpisodeDto>> SelectedEpisodesChanged { get; set; }

    [Parameter]
    public string Placeholder { get; set; } = string.Empty;

    [Parameter]
    public string Label { get; set; } = string.Empty;


    [Parameter]
    public List<EpisodeDto> SelectedEpisodes { get; set; }
    //{
    //    get => _selectedEpisodes;

    //    set
    //    {
    //        if (_selectedEpisodes != value) return;
    //        _selectedEpisodes = value;
    //        SelectedEpisodesChanged.InvokeAsync(value);
    //    }
    //}

    protected override async Task OnInitializedAsync()
    {
        await GetEpisodes();
    }

    protected string FormatEpisode(EpisodeDto episode)
    {
        return episode.Code;
    }

    protected Task OnSelectedEpisodesChanged(IEnumerable<EpisodeDto?>? episodes)
    {
        var clean = episodes?.Where(e => e is not null).Cast<EpisodeDto>().ToList() ?? [];
        SelectedEpisodes = clean;
        SelectedEpisodesChanged.InvokeAsync(clean);
        return Task.CompletedTask;
    }


    private async Task GetEpisodes()
    {
        Episodes = (await EpisodeDataProvider.GetAsync()).OrderBy(x => x.Code);
    }
}