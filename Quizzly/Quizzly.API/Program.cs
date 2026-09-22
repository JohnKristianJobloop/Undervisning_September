using Microsoft.AspNetCore.Mvc;
using Quizzly.API.Data;

// Minimal API: hele oppstarten av webserveren skjer her i Program.cs.
// builder samler opp konfigurasjon og tjenester FØR appen bygges.
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// OpenAPI/Swagger: lager en maskinlesbar beskrivelse av endepunktene våre.
builder.Services.AddOpenApi();
// Dependency injection: vi sier HVA vi trenger (IQuestionStore) og HVEM som leverer det
// (JsonQuestionStore). Endepunktene ber bare om interfacet, så lagringen kan byttes ut senere.
// Singleton = én instans for hele appen, den deles av alle forespørsler.
builder.Services.AddSingleton<IQuestionStore, JsonQuestionStore>();

// Etter Build() er tjenestelisten låst, og vi setter opp selve HTTP-pipelinen.
var app = builder.Build();

// Configure the HTTP request pipeline.
// OpenAPI-dokumentet eksponeres bare lokalt under utvikling, ikke i produksjon.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Middleware: sender http-forespørsler videre til https.
app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// Eksempel-endepunktet som følger med malen. MapGet kobler en URL til koden som svarer.
// Det du returnerer blir automatisk gjort om til JSON.
app.MapGet("/weatherforecast", async () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Run starter serveren og blokkerer helt til appen avsluttes.
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
