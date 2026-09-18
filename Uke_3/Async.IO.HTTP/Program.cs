using System.Net.Http.Json;
using Async.Common;
using Async.IO.HTTP;

// Ekte I/O: her venter vi faktisk på nettet og på en server vi ikke kontrollerer.
// HttpClient er ment å gjenbrukes, ikke lages på nytt for hvert kall.
var client = new HttpClient();

Log.Line("Kaller joke API");

Log.Line("Venter på OS nettverkskall");
// await her: tråden er fri mens pakkene er på reise.


// GET /joke/Any?blacklistFlags=nsfw,religious,political,racist,sexist,explicit&format=json&amount=10&type=twopart HTTP/1.1
// Host: https://v2.jokeapi.dev
// User-Agent: curl/8.6.0
// Accept: application/json


var response = await client.GetAsync("https://v2.jokeapi.dev/joke/Any?blacklistFlags=nsfw,religious,political,racist,sexist,explicit&format=json&amount=10&type=twopart");
Log.Line($"Fikk en response: {response.StatusCode}");
Log.Line("Serialiserer data til et Response objekt.");
// Også lesing av svaret er I/O, derfor await en gang til.
var collection = await response.Content.ReadFromJsonAsync<MultipleResponse>();
// Deserialisering kan gi null hvis JSON ikke passer, så vi sjekker før vi looper.
if (collection is not null) 
foreach (var value in collection.Jokes)
{
    Log.Line(value.Category);
    Log.Note(value.Setup);
    Log.Note(value.Delivery);
}
