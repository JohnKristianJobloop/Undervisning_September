using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using Async.Common;

namespace Async.IO.Blocking;

// Alt et program gjør utenfor seg selv, går gjennom operativsystemet:
// filer, nettverk, til og med Console.WriteLine. Det er her ventingen oppstår.
public static class Step1_OS
{
    // Ingen await her, men signaturen må passe menyen, derfor Task.CompletedTask til slutt.
    public static Task Run()
    {
        Log.Header("Step 1: Alt vi gjør, går via OS: ");

        Log.Line($"Vårt program er process {Environment.ProcessId}\n på maskinen {Environment.MachineName}");

        // Fil-I/O: vi eier ikke disken, vi ber OS om å gjøre jobben.
        var path = Path.Combine(Path.GetTempPath(), "async-demo.txt");
        Log.Line($"Vi kan be OS skrive til filen {path}");
        File.WriteAllText(path, $"skrevet av process {Environment.ProcessId}");
        
        Log.Line("Vi kan be OS om å lese filer");
        var content = File.ReadAllText(path);

        Log.Line($"OS leste: {content}");



        // Nettverks-I/O: port 0 betyr "OS, velg en ledig port til meg".
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        Log.Line($"ba OS om en port, fikk {port}");
        Log.Note("Andre programmer kan koble seg til oss, via denne porten.");
        listener.Stop(); // gi porten tilbake til OS med en gang

        Log.Line("Selv printing til consol er I/O, OS sender data til terminalen sin Input port.");
        return Task.CompletedTask;
    }
}