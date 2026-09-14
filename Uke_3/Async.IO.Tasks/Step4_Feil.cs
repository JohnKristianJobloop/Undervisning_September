using Async.Common;

namespace Async.IO.Tasks;

// I/O feiler oftere enn vanlig kode: nettet ryker, tjenesten svarer ikke, brukeren gir opp.
// try/catch fungerer som vanlig rundt await, og CancellationToken er måten vi sier "slutt".
public static class Step4_Feil
{
    public static async Task Run()
    {
        Log.Header("Step 4: feil og avbrudd");

        Log.Line("Kaller en metode som vil crashe");
        // 1) Et unntak i en async-metode blir liggende i Task-en, og kastes når vi await-er.
        Task<string> failing = FailingCall();
        Log.Line("kallet er startet, ingenting galt enda.");

        try
        {
            await failing;
        }
        catch (InvalidOperationException ex)
        {
            Log.Line($"await kastet unntaket: {ex.Message}");
        }

        // 2) WhenAll: alle kjører ferdig, men await kaster bare det første unntaket.
        Task[] all = { FailingCall(), SlowService.FetchAsync("ok", 200), FailingCall() };

        try
        {
            await Task.WhenAll(all);
        }
        catch (InvalidOperationException)
        {
            int failed = all.Count(task => task.IsFaulted);
            Log.Line($"WhenAll: {failed} av {all.Length} feilet");
            Log.Note("vil du ha alle feilene, se på task.Exception for hver enkelt");
        }

        // 3) Avbrudd: vi gir tjenesten 300 ms på seg.
        using CancellationTokenSource cts = new CancellationTokenSource(300);

        try
        {
            await SlowService.FetchAsync("treg tjeneste", 2000, cts.Token);
        }
        catch (OperationCanceledException)
        {
            Log.Line("vi ga opp etter fristen");
        }

        Log.Note("send token-en helt ned til I/O-kallet, ellers stopper ingenting");
    }

    private static async Task<string> FailingCall()
    {
        await Task.Delay(100);
        throw new InvalidOperationException("tjenesten svarte 500");
    }
}
