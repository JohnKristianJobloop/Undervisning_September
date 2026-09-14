using System.Net;
using Async.Common;

namespace Async.IO.Tasks;

// En Task er ikke arbeidet selv, den er kvitteringen på arbeid som pågår.
// await pakker ut resultatet når det er klart, og gir tråden fri i mellomtiden.
public static class Step1_Tasks
{
    public static async Task Run()
    {
        Log.Header("Step 1: Hva er en task?");

        Log.Line("Kaller SlowService.FetchAsync");
        // Kallet starter med en gang, vi får en Task tilbake uten å vente.
        var job = SlowService.FetchAsync("Price");
        Log.Line($"Fikk kvitering på task startet. Status {job.Status}");
        Log.Line("Rekker å gjøre andre ting mens jobben pågår.");
        var result = await job; // her, og først her, venter vi på svaret
        Log.Line($"await ga oss \"{result}\". Status: {job.Status}");
        Console.WriteLine();
        var flow = ShowAwaitFlow();
        Log.Line("Caller får tråd tilbake med en gang");
        await flow;

    }
    // Viser at en async-metode kjører synkront helt frem til første await.
    // Resten er en fortsettelse som plukkes opp når ventingen er over.
    private static async Task ShowAwaitFlow()
    {
        Log.Line("A: Før await, dette kjører med en gang på caller.");
        await Task.Delay(300);
        Log.Line("B: Etter await, resten av metoden plukket opp etterpå.");
    }
}