using System.Net;
using Async.IO.HTTP.Server.Models;

// HttpListener er OS-et sin egen HTTP-server, så vi trenger ikke noe rammeverk rundt.
var listener = new HttpListener();
// Prefikset bestemmer hvilken adresse og port OS-et skal lytte på for oss.
listener.Prefixes.Add("http://localhost:42069/");


var server = new HttpServer(listener);


try
{
    // Vi venter ikke på StartAsync, så serverløkken kjører videre mens vi står under her.
    server.StartAsync();
    Console.WriteLine("Press any key to stop");
    // ReadKey blokkerer denne tråden helt til brukeren velger å avslutte.
    Console.ReadKey();
}
finally
{
    // finally garanterer at porten frigis, uansett hvordan vi forlater try-blokken.
    server.Stop();
}
