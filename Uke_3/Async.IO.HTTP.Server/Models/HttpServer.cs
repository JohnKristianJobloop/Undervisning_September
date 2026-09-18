using System.Net;
using System.Text;
using System.Text.Json;
using Async.Common;

namespace Async.IO.HTTP.Server.Models;


// Listeneren sendes inn utenfra, slik at serveren slipper å bestemme adresse og port selv.
public class HttpServer(HttpListener listener)
{
    private HttpListener _listener = listener;
    // Vitsene lever kun i minnet, så alt forsvinner når serveren stoppes.
    private List<string> Jokes {get;set;} = []; 

    public async Task StartAsync()
    {
        // Start() åpner porten, men henter ikke inn forespørsler av seg selv.
        _listener.Start();

        // Serveren kjører til Stop() kalles og GetContextAsync kaster.
        while (true)
        {
            // Await her: tråden er fri mens vi venter på at en klient skal koble seg til.
            var context = await _listener.GetContextAsync();

            // Enkel ruting: vi ser bare på HTTP-metoden, ikke på URL-en.
            switch (context.Request.HttpMethod)
            {
                case "GET":
                    await ProcessGetRequest(context);
                    break;
                case "POST":
                    await ProcessPostRequest(context);
                    break;
            }
        }
    }

    private async Task ProcessGetRequest(HttpListenerContext context)
    {
        Log.Line($"Processing request from {context.Request.UserHostAddress}");
        // Nettverket sender bytes, så listen må serialiseres til JSON og kodes til UTF-8.
        var data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(Jokes));
        // Klienten må vite hvor mange bytes den skal lese før den slutter å vente.
        context.Response.ContentLength64 = data.Length;
        context.Response.ContentEncoding = Encoding.UTF8;
        // ContentType forteller klienten at svaret skal tolkes som JSON.
        context.Response.ContentType = "application/json";
        // using lukker strømmen når metoden er ferdig, og da er svaret sendt.
        using var output = context.Response.OutputStream;
        // Også skrivingen ut på nettverket er I/O, derfor await.
        await output.WriteAsync(data);
    }

    private async Task ProcessPostRequest(HttpListenerContext context)
    {
        Log.Line($"Processing request from {context.Request.UserHostAddress}");
        var request = context.Request;
        var response = context.Response;

        // Uten body har vi ingen vits å lagre.
        if (request.HasEntityBody)
        {
            using var stream = request.InputStream;
            // ContentLength64 sier hvor stor bufferen må være for å romme hele body-en.
            byte[] buffer = new byte[request.ContentLength64];
            // ReadExactlyAsync venter til hele bufferen er fylt, ikke bare det som har rukket å komme.
            await stream.ReadExactlyAsync(buffer);
            // Bytene tolkes tilbake til tekst med samme koding som klienten brukte.
            var joke = Encoding.UTF8.GetString(buffer);
            Jokes.Add(joke);
            // 201 Created forteller klienten at vi opprettet noe nytt.
            response.StatusCode = (int)HttpStatusCode.Created;
        }
        // 400 Bad Request: forespørselen var feil fra klienten sin side, ikke vår.
        else response.StatusCode = (int)HttpStatusCode.BadRequest;
        // Close() sender statuskoden av gårde og avslutter svaret.
        response.Close();
        
    }

    public void Stop()
    {
        // Stop() slutter å ta imot nye forespørsler, Close() frigir porten.
        _listener.Stop();
        _listener.Close();
    }
}
