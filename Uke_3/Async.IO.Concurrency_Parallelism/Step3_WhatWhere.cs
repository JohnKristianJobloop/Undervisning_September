using System.Diagnostics;
using Async.Common;

namespace Async.IO.Concurrency_Parallelism;

// Regelen er kort:
//
//   arbeidet venter på noe (I/O)   ->  async / await + Task.WhenAll
//   arbeidet bruker CPU            ->  Task.Run / Parallel
//
// Her måler vi hva som skjer når man velger feil.
public static class Step3_WhatWhere
{
    public static async Task Run()
    {
        Log.Header("Steg 3: hva passer til hva");

        Stopwatch watch = Stopwatch.StartNew();

        // I/O + async: riktig verktøy.
        watch.Restart();
        await Task.WhenAll(Enumerable.Range(0, 4).Select(i => SlowService.FetchAsync($"tj-{i}", 400)));
        Log.Line($"I/O med WhenAll:        {watch.ElapsedMilliseconds,5} ms   (riktig)");

        // I/O + tråder: samme tid, men vi låner fire tråder til å stå og vente.
        watch.Restart();
        Parallel.For(0, 4, i => SlowService.Fetch($"tj-{i}", 400));
        Log.Line($"I/O med Parallel.For:   {watch.ElapsedMilliseconds,5} ms   (virker, men sløser med tråder)");

        // CPU + async: ingen gevinst. async flytter ikke arbeid til flere kjerner.
        watch.Restart();
        await Task.WhenAll(Enumerable.Range(0, 4).Select(_ => FakeAsyncCpu()));
        Log.Line($"CPU med await:          {watch.ElapsedMilliseconds,5} ms   (ingen gevinst)");

        // CPU + Parallel: riktig verktøy.
        watch.Restart();
        Parallel.For(0, 4, _ => CpuWork.CountPrimes());
        Log.Line($"CPU med Parallel.For:   {watch.ElapsedMilliseconds,5} ms   (riktig)");

        Log.Note("async gir ikke fart - det gir ledige tråder. Parallell gir fart.");
    }

    // Ser asynkron ut, men gjør bare CPU-arbeid. Den blokkerer tråden like fullt.
    private static async Task FakeAsyncCpu()
    {
        CpuWork.CountPrimes();
        await Task.CompletedTask;
    }
}
