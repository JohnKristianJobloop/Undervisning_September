using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Quizzly.API.Models;
using Quizzly.Core.Models;

namespace Quizzly.API.Data;

// Database-implementasjonen av IQuestionStore. Resten av appen merker ingen forskjell fra JSON-varianten.
// Vi bruker en factory fordi klassen er singleton, mens en DbContext skal leve kort (én per operasjon).
public sealed class SqliteQuestionStore(
    IDbContextFactory<QuizDbContext> contextFactory,
    ILogger<SqliteQuestionStore> logger
): IQuestionStore
{
    // SQLite sin feilkode for brudd på en UNIQUE-regel.
    private const int SqliteConstraintUnique = 2067;
    public async Task<IReadOnlyList<Question>> LoadAllAsync()
    {
        await using QuizDbContext db = await contextFactory.CreateDbContextAsync();

        // AsNoTracking: vi skal bare lese, så EF trenger ikke holde øye med endringer.
        var entities = await db.Questions.AsNoTracking().OrderBy(q => q.Id).ToListAsync();
        logger.LogInformation($"Fetched {entities.Count} questions from db");
        // Oversett fra databasemodell til domenemodell før vi sender dataen videre.
        return [..entities.Select(e => e.ToQuestion())];
    }

    public async Task<bool> TryAddAsync(Question question)
    {
        ArgumentNullException.ThrowIfNull(question);

        await using var db = await contextFactory.CreateDbContextAsync();

        // Sjekk om spørsmålet finnes fra før (uavhengig av store/små bokstaver).
        var normalizedText = QuestionEntity.Normalize(question.Text);
        if (await db.Questions.AnyAsync(q => q.NormalizedText == normalizedText))
        {
            return false; 
        }

        db.Questions.Add(QuestionEntity.From(question));
        try
        {
            await db.SaveChangesAsync();
        }
        // Sikkerhetsnett: hvis to forespørsler legger inn samme spørsmål samtidig,
        // stopper den unike indeksen den andre. Det behandler vi som "finnes allerede".
        catch (DbUpdateException exception) when (exception.InnerException is SqliteException {SqliteExtendedErrorCode: SqliteConstraintUnique})
        {
            return false;
        }
        logger.LogInformation("Nytt spørsmål skrevet til databasen.");
        return true;
    }
}
