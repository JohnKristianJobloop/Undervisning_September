using Quizzly.API.Data;
using Quizzly.API.Models;
using Quizzly.Core.Interfaces;
using Quizzly.Core.Models;

namespace Quizzly.API.Services;


// Applikasjonslogikken mellom endepunktene og domenet: den vet HVA som skal skje
// når noen svarer, mens endepunktene bare vet hvordan svaret sendes over HTTP.
// Primærkonstruktør: køen, lageret og loggeren kommer inn via DI.
public sealed class ReviewQueueService(
    IReviewQueue<Question> queue,
    IQuestionStore store,
    ILogger<ReviewQueueService> logger
)
{
    // Tjenesten er singleton, så flere forespørsler kan treffe køen samtidig.
    // Lock slipper én tråd om gangen inn, så køen ikke endres midt i en operasjon.
    private readonly Lock _gate = new();

    // Prioritet: lavt tall = tidlig i køen. Nye spørsmål kommer først,
    // spørsmål du svarte feil på legges lenger bak og dukker opp igjen senere.
    private const int NewPriority = 1;
    private const int ReviewPriority = 4;

    // Leser av køen uten å endre den: hvor langt er det igjen, og hva er neste spørsmål.
    public ReviewQueueStatusResponse GetStatus()
    {
        lock (_gate)
        {
            // Peek på tom kø kaster, så vi sjekker IsEmpty først.
            PendingQuestionResponse? next = queue.IsEmpty
            ? null
            : PendingQuestionResponse.From(queue.Peek());

            return new(queue.Count, queue.IsEmpty, next);
        }
    }

    // Fyller køen med spørsmålene fra lageret (JSON-filen).
    public async Task<int> ReloadFromStoreAsync()
    {
        // await utenfor lock: vi kan ikke vente på disk mens vi holder låsen.
        var questions = await store.LoadAllAsync();

        lock (_gate)
        {
            foreach (var question in questions)
            {
                queue.Enqueue(question, NewPriority);
            }

            logger.LogInformation($"Loaded {questions.Count} questions into ReviewQueue");
            
            return queue.Count;
        }

    }

    // Hele svarrunden i én metode: valider, ta ut av køen, sjekk fasit,
    // og legg spørsmålet tilbake hvis det ble feil.
    public AnswerResult SubmitAnswer(int answer)
    {
        lock (_gate)
        {
            // Ingenting igjen å svare på - endepunktet gjør dette om til 404.
            if (queue.IsEmpty)
            {
                return new AnswerResult(AnswerStatus.QueueEmpty);
            }

            // Peek, ikke Dequeue: et ugyldig svar skal ikke koste deg spørsmålet.
            var question = queue.Peek();

            if (!question.IsValidAnswer(answer))
            {
                // Vi sender med antall alternativer så feilmeldingen kan si "velg 1 til 4".
                return new AnswerResult(AnswerStatus.InvalidAnswer, AlternativesCount: question.Alternatives.Count);
            }

            // Først nå er svaret gyldig, og spørsmålet kan tas ut av køen.
            queue.Dequeue();

            question.UserAnswer = answer;

            // Feil svar = spørsmålet legges tilbake med lavere prioritet, og kommer igjen senere.
            if (!question.IsCorrect)
            {
                queue.Enqueue(question, ReviewPriority);
            }

            // Fasiten sendes først NÅ, etter at brukeren har svart.
            var response = new AnswerResponse(
                question.Text,
                question.IsCorrect,
                question.CorrectAnswer,
                question.CorrectAnswerText,
                queue.Count
            );
            return new AnswerResult(AnswerStatus.Answered, response);
        }
    }
}
