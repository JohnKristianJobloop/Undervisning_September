using System.Diagnostics;
using Async.Common;

namespace Async.IO.Tasks;

// Vanligste misforståelsen: await i en løkke er fremdeles ett kall om gangen.
// Vi slipper å blokkere tråden, men totaltiden blir den samme som synkront.
public static class Step2_AwaitNotParallel
{
    public static async Task Run()
    {
        Log.Header("Step 2: Await er ikke det samme som \"parallelt\".");
        string[] services = ["Lager", "Pris", "Frakt"];

        var watch = Stopwatch.StartNew();
        foreach(var service in services) SlowService.Fetch(service); // synkront: ca. 3 x 500 ms
        Log.Line($"Synkront uten await: {watch.ElapsedMilliseconds} ms");
        watch.Restart();

        // Samme totaltid: vi await-er før vi starter neste kall.
        foreach(var serivce in services) 
        {
            //Start
            await SlowService.FetchAsync(serivce);
            //Continuation
        };


        Log.Line($"Await i løkke: {watch.ElapsedMilliseconds} ms");
    }
}