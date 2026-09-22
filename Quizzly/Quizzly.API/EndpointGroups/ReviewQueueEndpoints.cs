using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Quizzly.API.Models;
using Quizzly.API.Services;

namespace Quizzly.API.EndpointGroups;


// Alle endepunktene for review-køen samlet i én fil, i stedet for i Program.cs.
// Program.cs sier bare "map disse", så oppstarten holder seg kort og lesbar.
public static class ReviewQueueEndpoints
{
    // extension-blokk: gir oss MapReviewQueueEndpoints() rett på IEndpointRouteBuilder.
    extension(IEndpointRouteBuilder routes)
    {
        public IEndpointRouteBuilder MapReviewQueueEndpoints()
        {
            // En gruppe deler felles prefiks, så vi slipper å gjenta "/review-queue" i hver rute.
            var group = routes.MapGroup("/review-queue");
            // GET = les, uten å endre noe. [FromServices] henter tjenesten fra DI-containeren.
            group.MapGet("", ([FromServices] ReviewQueueService reviewQueue) => TypedResults.Ok(reviewQueue.GetStatus()))
                .WithName("GetReviewQueue")
                .WithSummary("Hvor mange spørsmål er igjen i køen, hva er neste spørsmål i køen");
            // POST = endrer tilstand. AnswerRequest leses automatisk fra JSON-bodyen.
            group.MapPost("/answer", (AnswerRequest request, [FromServices] ReviewQueueService reviewQueue) =>
            {
                var result = reviewQueue.SubmitAnswer(request.Answer);
                // Endepunktets jobb: oversette utfallet fra tjenesten til riktig HTTP-statuskode.
                return result.status switch
                {
                    AnswerStatus.Answered => Results.Ok(result.response),
                    // Tom kø er ikke en feil i koden, men "det finnes ikke noe her" -> 404.
                    AnswerStatus.QueueEmpty => Results.Problem(
                        "Køen er tom, ingen ting igjen å svare på",
                        statusCode: StatusCodes.Status404NotFound
                    ),
                    // Siste utfall: klienten sendte et ugyldig tall -> 400 med forklaring på hva som er lov.
                    _ => Results.ValidationProblem(new Dictionary<string, string[]>
                    {
                        ["answer"] = [$"svaret må være et tall mellom 1 og {result.AlternativesCount}"]
                    })
                };
            })
            .WithName("AnswerNextQuestion")
            .WithSummary("Svarer på det første spørsmålet i kø, feil svar pusher tilbake i kø");

            // async fordi den leser fra fil: tråden frigjøres mens vi venter på disken.
            group.MapPost("/reset", async ([FromServices] ReviewQueueService service) =>
            {
                int count = await service.ReloadFromStoreAsync();
                return Results.Ok(new {Loaded = count });
            })
            .WithName("ResetReviewQueue")
            .WithSummary("Legger inn alle spørsmål fra JSON tilbake i Queue");
            
            // Returnerer routes så kallene kan kjedes i Program.cs.
            return routes;
        }
    }
}
