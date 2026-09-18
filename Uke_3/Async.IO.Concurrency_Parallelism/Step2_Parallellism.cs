using System.Diagnostics;
using Async.Common;

namespace Async.IO.Concurrency_Parallelism;

// Parallellisme handler om CPU-arbeid. Her finnes det ingen venting å utnytte -
// vi må ha flere kjerner i gang for å bli fortere ferdige.
public static class Step2_Parallellism
{
    private const int Jobs = 16;

    public static Task Run()
    {
        Log.Header("Steg 2: parallellisme - flere kjerner i arbeid");

        CpuWork.CountPrimes(1000);   // varm opp JIT-en før vi måler

        // Sekvensielt: én kjerne gjør alt.
        Stopwatch watch = Stopwatch.StartNew();
        for (int i = 0; i < Jobs; i++)
        {
            CpuWork.CountPrimes();
        }
        long sequential = watch.ElapsedMilliseconds;
        Log.Line($"sekvensielt: {sequential} ms");

        // Parallelt: Parallel.For fordeler jobbene over kjernene.
        watch.Restart();
        Parallel.For(0, Jobs, _ => CpuWork.CountPrimes());
        long parallel = watch.ElapsedMilliseconds;
        Log.Line($"parallelt:   {parallel} ms på {Environment.ProcessorCount} kjerner");

        Log.Note($"omtrent {(double)sequential / Math.Max(parallel, 1):0.0} ganger raskere");
        Log.Note("her hjelper async ingenting - det er ingen venting å gi fra seg");
        return Task.CompletedTask;
    }
}
