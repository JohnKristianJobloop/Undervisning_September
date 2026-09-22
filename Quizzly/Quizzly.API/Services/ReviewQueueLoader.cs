namespace Quizzly.API.Services;

// En hosted service er kode som kjører i takt med selve appen, ikke i takt med en forespørsel.
// Her bruker vi den til å fylle køen én gang ved oppstart, så den første brukeren
// slipper å vente på at JSON-filen leses.
internal sealed class ReviewQueueLoader(
    ReviewQueueService service,
    ILogger<ReviewQueueLoader> logger
) : IHostedService
{
    // Kjøres automatisk av ASP.NET når appen starter.
    public async Task StartAsync(CancellationToken token)
    {
        int count = await service.ReloadFromStoreAsync();
        logger.LogInformation($"{count} questions loaded from hosted service!");
    }

    // Vi har ingenting å rydde opp i ved nedstenging, så vi returnerer en ferdig Task.
    public Task StopAsync(CancellationToken token) => Task.CompletedTask;

}
