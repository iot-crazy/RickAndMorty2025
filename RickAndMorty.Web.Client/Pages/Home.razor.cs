using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using RickAndMorty.Contracts;
using RickAndMorty.DTO.Character;

namespace RickAndMorty.Web.Client.Pages;

public class HomeBase : ComponentBase
{
    [Inject]
    private IApiCharacterDataProvider CharacterProvider { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;


    protected List<CharacterDto> AllCharacters = [];
    protected bool ImportInProgress = false;
    protected string CurrentStatus = "Starting...";

    private HubConnection? _hubConnection;

    protected override async Task OnInitializedAsync()
    {
        _hubConnection = new HubConnectionBuilder()
                    .WithUrl(NavigationManager.ToAbsoluteUri("/importhub"))
                    .WithAutomaticReconnect()
                    .Build();

        _hubConnection.On<string>("ProgressChanged", message =>
        {
            _ = SetStatusMessage(message);
        });

        _hubConnection.On("ImportComplete", ImportComplete);

        await _hubConnection.StartAsync();

        await GetAllCharacters();
        StateHasChanged();
    }

    protected async Task ImportData()
    {
        ImportInProgress = true;
        await InvokeAsync(StateHasChanged);

        if (_hubConnection == null)
        {
            await SetStatusMessage("SignalR not configured.");
            return;
        }

        if (_hubConnection.State == HubConnectionState.Disconnected)
        {
            try
            {
                await SetStatusMessage("Reconnecting to SignalR...");
                await _hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                await SetStatusMessage($"Failed to connect: {ex.Message}");
                return;
            }
        }

        try
        {
            await SetStatusMessage("Connected! Starting import...");
            await _hubConnection.SendAsync("StartImport");
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task ImportComplete()
    {
        await SetStatusMessage("All done! Checking status...");
        await GetAllCharacters();
        ImportInProgress = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task SetStatusMessage(string message)
    {
        CurrentStatus = message;
        await InvokeAsync(StateHasChanged);
    }

    private async Task GetAllCharacters()
    {
        AllCharacters = (await CharacterProvider.GetAsync()).ToList();
    }

}

