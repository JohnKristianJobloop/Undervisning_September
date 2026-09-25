using Microsoft.VisualBasic;
using Quizzly.API.Models;
using Quizzly.API.Services;

namespace Quizzly.API.EndpointGroups;

// Endepunkter for å jobbe med selve spørsmålene (datasettet), adskilt fra review-køen.
public static class QuestionEndpoints
{
    extension(IEndpointRouteBuilder routes)
    {
        public IEndpointRouteBuilder MapQuestionEndpoints()
        {
            // Alle endepunkter i gruppen får /questions foran seg.
            var group = routes.MapGroup("/questions");

            // POST /questions: legg til et nytt spørsmål.
            group.MapPost("", async (
                NewQuestionRequest request,
                ReviewQueueService queue
            ) =>
            {
                // Valider først – ugyldig input gir 400 med en liste over hva som er feil.
                Dictionary<string, string[]> errors = request.Validate();

                if (errors.Count > 0)
                {
                    return Results.ValidationProblem(errors);
                }

                var question = request.ToQuestion();

                // false betyr at spørsmålet finnes fra før → 409 Conflict.
                if (!await queue.TryAddQuestionAsync(question))
                {
                    return Results.Problem(
                        $"Spørsmålet \"{question.Text}\" finnes allerede",
                        statusCode: StatusCodes.Status409Conflict);
                }
                // 201 Created sammen med spørsmålet slik det ble lagret.
                return Results.Created("/questions", QuestionResponse.From(question));
            })
            .WithName("AddQuestion")
            .WithSummary("Legger et nytt spørsmål i datasettet");

            return group;
        }
    }
}
