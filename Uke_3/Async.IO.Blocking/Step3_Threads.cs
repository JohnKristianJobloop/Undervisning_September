using System.Data;
using System.Diagnostics;
using System.Net.Sockets;
using Async.Common;

namespace Async.IO.Blocking;

// Det nærliggende svaret på blokkering er "bruk flere tråder". Det virker,
// men hver tråd koster minne og planlegging i OS. Her ser vi prisen.
public static class Step3_Threads
{
    public const int Operations = 50;

    public static async Task Run()
    {
        Log.Header("Step 3: Spre arbeidet ut på flere tråder");

        var before = Log.ThreadCount; // sammenlign med tallet mens alt kjører
        Log.Line($"Tråder før vi starter: {before}");
        var watch = Stopwatch.StartNew();
        List<Thread> threads = [];

        // A: én egen tråd per operasjon. 50 tråder som alle bare venter.
        for (var i = 0; i < Operations; i++)
        {
            var thread = new Thread(()=>SlowService.Fetch("service"));
            thread.Start();
            threads.Add(thread);
        }
        Log.Line($"Threads i processen nå: {Log.ThreadCount}");
        foreach(var thread in threads) thread.Join(); // Join blokkerer til tråden er ferdig
        Log.Line($"A: {Operations} waits på egene tråder tok {watch.ElapsedMilliseconds} ms");

        watch.Restart();
        // B: samme arbeid som Task-er. Ingen nye tråder, ventingen ligger hos OS.
        List<Task> tasks = [];
        for (var i = 0; i < Operations; i++)
        {
            tasks.Add(SlowService.FetchAsync("service"));
        }
        Log.Line($"Threads i processen mens alle {Operations} holder på: {Log.ThreadCount}");
        await Task.WhenAll(tasks); // venter på alle uten å holde på en tråd

        Log.Line($"B: {Operations} waits asynkront tok {watch.ElapsedMilliseconds} ms");

        Log.Note("Tasks flytter ansvaret på venting til OS,\n\tistedenfor en Thread hvor vårt program fremdeles har ansvar for venting.");
    }
}