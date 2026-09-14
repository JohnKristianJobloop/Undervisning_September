using System.Diagnostics;
using Async.Common;

namespace Async.IO.Tasks;


// Fiksen på steg 2: start alle kallene først, await etterpå.
// Da overlapper ventingen, og totaltiden blir omtrent lik det tregeste kallet.
public static class Step3_StartAll
{
    public static async Task Run()
    {
        Log.Header("Steg 3: Start alt først, vent etterpå");

        var watch = Stopwatch.StartNew();

        // Ingen await her: alle tre er i gang samtidig.
        var stock = SlowService.FetchAsync("Lager");
        var price = SlowService.FetchAsync("Price", 700);
        var shipping = SlowService.FetchAsync("Frakt", 300);

        Log.Line("Alle tre startet");

        // WhenAll: vent til alle er ferdige, svarene kommer i samme rekkefølge som vi sendte inn.
        var answers = await Task.WhenAll(stock,price,shipping);
        Log.Line($"Alle svar tilbake etter {watch.ElapsedMilliseconds} ms: {string.Join(", ", answers)}");

        Console.WriteLine();
        watch.Restart();

        Log.Line("WhenAny()");
        var mirrorA = SlowService.FetchAsync("Speil-A", 800);
        var mirrorB = SlowService.FetchAsync("Speil-B", 200);

        // WhenAny: gir oss Task-en som ble ferdig først. Det ytre await gir Task-en,
        // det indre await under gir selve svaret.
        var first = await Task.WhenAny(mirrorA, mirrorB);
        Log.Line($"Første svar etter {watch.ElapsedMilliseconds} ms: {await first}");
        // Taperen kjører videre. Await den også, ellers kan feil forsvinne i stillhet.
        await Task.WhenAll(mirrorA, mirrorB);
        Log.Line("Rydd opp, husk å await andre task.");
        Log.Note("Tregeste kjører kanskje fremdeles.");
    }
}