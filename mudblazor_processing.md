# Processing 

```csharp
@page "/message-processor"
@using System.Threading.Channels
@inject ILogger<MessageProcessor> Logger

<MudPaper Class="p-6 max-w-3xl mx-auto mt-10">
    <MudText Typo="Typo.h5">Long-running Task with Live Message Processing</MudText>
    <MudDivider Class="my-4" />
    
    <MudButton Variant="Variant.Filled" Color="Color.Primary" OnClick="StartProcessing" Disabled="@_isRunning">
        @(_isRunning ? "Processing..." : "Start Task")
    </MudButton>
    
    @if (_isRunning)
    {
        <MudProgressLinear Color="Color.Primary" Indeterminate="true" Class="my-4" />
    }

    <MudPaper Class="p-4 mt-4 bg-grey-lighten-4 rounded-lg overflow-auto" Style="height: 300px;">
        @foreach (var message in _messages)
        {
            <MudText Typo="Typo.body2">@message</MudText>
        }
    </MudPaper>
</MudPaper>

@code {
    private List<string> _messages = new();
    private bool _isRunning = false;
    private CancellationTokenSource? _cts;

    private async Task StartProcessing()
    {
        if (_isRunning) return;

        _isRunning = true;
        _messages.Clear();
        _cts = new();

        var channel = Channel.CreateUnbounded<string>();

        // Task: Produces messages
        var producer = Task.Run(async () =>
        {
            try
            {
                for (int i = 1; i <= 10; i++)
                {
                    await channel.Writer.WriteAsync($"Step {i}: Processing...", _cts.Token);
                    await Task.Delay(800, _cts.Token); // Simulate work
                }
                await channel.Writer.WriteAsync("✅ Task Completed Successfully!", _cts.Token);
            }
            catch (OperationCanceledException)
            {
                await channel.Writer.WriteAsync("⚠️ Task was cancelled.");
            }
            finally
            {
                channel.Writer.Complete();
            }
        });

        // Task: Consumes messages and updates UI
        var consumer = Task.Run(async () =>
        {
            await foreach (var message in channel.Reader.ReadAllAsync())
            {
                _messages.Add($"{DateTime.Now:T} — {message}");
                StateHasChanged();
            }
        });

        await Task.WhenAll(producer, consumer);
        _isRunning = false;
        StateHasChanged();
    }
}
```
