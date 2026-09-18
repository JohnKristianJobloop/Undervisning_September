using Async.Common;

namespace Async.IO.Concurrency_Parallelism;

// Concurrency handler om å ha flere oppgaver i gang. Hver gang en oppgave venter på I/O,
// kan en annen komme videre. Legg merke til at utskriften flettes sammen, og at
// antall tråder holder seg lavt - oppgavene deler på de samme trådene.
public static class Step1_Concurrency
{
    public static async Task Run()
    {
        Log.Header("Steg 1: concurrency, mange oppgaver, få tråder");

        Log.Line($"tråder før: {Log.ThreadCount}");

        Task[] orders =
        {
            HandleOrder("A", 200),
            HandleOrder("B", 120),
            HandleOrder("C", 300)
        };

        await Task.WhenAll(orders);

        Log.Line($"tråder etter: {Log.ThreadCount}");
        Log.Note("ingen av ordrene ventet på hverandre, og ingen tråd sto stille");
    }

    // Tre steg som hver venter på noe utenfor programmet.
    private static async Task HandleOrder(string id, int ms)
    {
        Log.Line($"ordre {id}: henter kunde");
        await SlowService.FetchAsync("kunder", ms);

        Log.Line($"ordre {id}: sjekker lager");
        await SlowService.FetchAsync("lager", ms);

        Log.Line($"ordre {id}: lagrer");
        await SlowService.FetchAsync("database", ms);

        Log.Line($"ordre {id}: ferdig");
    }
}
