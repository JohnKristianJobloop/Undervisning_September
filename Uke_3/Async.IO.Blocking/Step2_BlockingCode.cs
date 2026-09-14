using Async.Common;

namespace Async.IO.Blocking;


// Synkron I/O: tråden står stille og venter på svar. Tidene legger seg oppå hverandre,
// tre kall på 500 ms tar 1500 ms selv om ingen av dem bruker CPU.
public class Step2_BlockingCode
{
    public static Task Run()
    {
        Log.Header("Step 2: Synkron I/O blokkerer tråden til programmet.");
        string[] services = ["Lager", "Pris", "Frakt"];
        // Ett kall om gangen: neste tjeneste blir ikke spurt før den forrige har svart.
        foreach(var service in services)
        {
            Log.Line($"spør {service}...");
            var answer = SlowService.Fetch(service);
            Log.Line($"fikk {answer}");
        }
        
        Log.Line($"ferdig etter {Log.ElapsedMs} ms");
        return Task.CompletedTask;
    }
}