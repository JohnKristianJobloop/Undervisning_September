using Quizzly.API.Data;
using Quizzly.API.EndpointGroups;
using Quizzly.API.Extensions;

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
builder.Services.AddReviewQueue(builder.Configuration);
builder.Services.AddSwaggerGen();

// Etter Build() er tjenestelisten låst, og vi setter opp selve HTTP-pipelinen.
var app = builder.Build();

// Configure the HTTP request pipeline.
// OpenAPI-dokumentet eksponeres bare lokalt under utvikling, ikke i produksjon.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware: sender http-forespørsler videre til https.
app.UseHttpsRedirection();

app.MapReviewQueueEndpoints();

// Endepunkter for å legge til nye spørsmål.
app.MapQuestionEndpoints();

// Run starter serveren og blokkerer helt til appen avsluttes.
app.Run();
