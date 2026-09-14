using System.Diagnostics;

namespace Async.Common;


// Felles utskrift for demoene. Viser alltid tid og tråd-id, slik at vi ser
// BÅDE når noe skjedde og hvilken tråd som gjorde det.
public static class Log
{
    // Måler tid siden forrige Header/Reset, ikke siden programstart.
    private static readonly Stopwatch Clock = Stopwatch.StartNew();

    public static void Reset() => Clock.Restart();

    public static long ElapsedMs => Clock.ElapsedMilliseconds;

    // Hovedlinjen: [tid] [tråd] melding. Tråd-id som endrer seg etter et await
    // er selve poenget i flere av stegene.
    public static void Line(string message)
    {
        Console.WriteLine($"[{ElapsedMs, 5} ms] [Thread: {Environment.CurrentManagedThreadId, 3}] {message}");
    }

    // Starter en ny seksjon og nullstiller klokken, så hvert steg måles for seg.
    public static void Header(string title)
    {
        Console.WriteLine();
        Console.WriteLine(title);
        Console.WriteLine(new string('-', title.Length));
        Reset();
    }

    // Innrykket linje for poenget vi vil at tilhøreren skal sitte igjen med.
    public static void Note(string message)
    {
        Console.WriteLine($"    -> {message}");
    }
    // Antall tråder OS har gitt prosessen akkurat nå. Brukes til å vise at
    // én tråd per operasjon skalerer dårlig.
    public static int ThreadCount => Process.GetCurrentProcess().Threads.Count;
}