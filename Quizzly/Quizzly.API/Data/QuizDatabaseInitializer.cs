using Microsoft.EntityFrameworkCore;
using Quizzly.API.Models;

namespace Quizzly.API.Data;


// Kjører én gang ved oppstart: fyller en tom database med spørsmålene fra JSON-filen (seeding).
internal sealed class QuizDatabaseInitializer(
    IDbContextFactory<QuizDbContext> contextFactory,
    ILogger<QuizDatabaseInitializer> logger,
    JsonQuestionStore store
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // await using: DbContext-en ryddes bort (og tilkoblingen lukkes) når metoden er ferdig.
        await using QuizDbContext db = await contextFactory.CreateDbContextAsync(cancellationToken);

        // Finnes det allerede spørsmål, har vi seedet før – da gjør vi ingenting.
        if (await db.Questions.AnyAsync(cancellationToken))
        {
            logger.LogInformation("No preseeding nessesary.");
            return;
        }

        // Les fra JSON, gjør om til entiteter og lagre alt i én operasjon.
        var questions = await store.LoadAllAsync();
        db.Questions.AddRange(questions.Select(QuestionEntity.From));
        await db.SaveChangesAsync(cancellationToken);
    }

    // Ingenting å rydde opp når appen stopper.
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
