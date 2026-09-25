using System.Text.Json;
using Microsoft.Extensions.Options;
using Quizzly.Core.Models;

namespace Quizzly.API.Data;


// Implementasjonen av IQuestionStore som leser spørsmålene fra en JSON-fil.
// sealed = ingen kan arve fra klassen, den er ferdig som den er.
public sealed class JsonQuestionStore : IQuestionStore
{
    // Innstillinger for JSON: camelCase ut, og vi godtar hvilken som helst store/små bokstaver inn.
    private static JsonSerializerOptions _serializerOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };
    // En webserver håndterer mange forespørsler samtidig. Semaforen med plass til 1
    // slipper én om gangen inn til filen, så vi ikke leser og skriver oppå hverandre.
    private readonly SemaphoreSlim _gate = new(1,1);
    private readonly string _filePath;
    private readonly ILogger<JsonQuestionStore> _logger;

    // Alt klassen trenger kommer inn via konstruktøren - DI-containeren fyller ut parameterne.
    public JsonQuestionStore(
        IOptions<QuestionStoreOptions> options,
        IHostEnvironment environment,
        ILogger<JsonQuestionStore> logger
    )
    {
        // ContentRootPath er prosjektmappen. Kombinert med den relative stien fra options
        // får vi en full sti som virker uansett hvor appen startes fra.
        _filePath = Path.Combine(environment.ContentRootPath, options.Value.FilePath);
        _logger = logger;
    }

    // Leser alle spørsmålene og gir dem tilbake som domenemodeller fra Core.
    public async Task<IReadOnlyList<Question>> LoadAllAsync()
    {
        // await her betyr "vent på tur", men uten å blokkere tråden.
        await _gate.WaitAsync();
        try
        {
            List<QuestionRecord> record = await ReadRecordsAsync();
            _logger.LogInformation($"Found {record.Count} questions");
            // Oversetter fra lagringsformat til domenemodell før resten av appen får se dataene.
            return [..record.Select(r => r.ToQuestion())];
        }
        finally
        {
            // finally kjører også hvis lesingen kaster. Uten Release ville køen stått fast for alltid.
            _gate.Release();
        }
    }

    public async Task<bool> TryAddAsync(Question question)
    {
        ArgumentNullException.ThrowIfNull(question);

        // Låsen sørger for at bare én forespørsel leser og skriver filen om gangen.
        await _gate.WaitAsync();

        try
        {
            // Les hele filen, sjekk for duplikat, legg til og skriv alt tilbake.
            var questions = await ReadRecordsAsync();
            if(questions.Any(q => Matches(q, question)))
            {
                return false;
            }
            questions.Add(QuestionRecord.From(question));
            await WriteRecordsAsync(questions);
            _logger.LogInformation($"Jsondata oppdatert. Datasett er nå {questions.Count} spørsmål.");

            return true;
        }
        finally
        {
            // finally: låsen slippes alltid, også hvis noe kaster en exception.
            _gate.Release();
        }
    }

    // Privat hjelpemetode: den eneste som faktisk rører filen.
    private async Task<List<QuestionRecord>> ReadRecordsAsync()
    {
        // Ingen fil er ikke en krasj - en tom liste er et helt gyldig svar.
        if (!File.Exists(_filePath))
        {
            _logger.LogWarning($"No file found at {_filePath}");
            return [];
        }
        // await using lukker filstrømmen når vi er ferdige, også hvis det oppstår en feil.
        await using FileStream stream = File.OpenRead(_filePath);
        // ?? [] fordi Deserialize kan returnere null (f.eks. hvis filen inneholder "null").
        return await JsonSerializer.DeserializeAsync<List<QuestionRecord>>(stream, _serializerOptions) ?? [];
    }
    // File.Create overskriver filen, så vi skriver alltid hele listen på nytt.
    private async Task WriteRecordsAsync(List<QuestionRecord> record)
    {
        await using FileStream stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, record, _serializerOptions);
    }
    
    // To spørsmål regnes som like hvis teksten er lik, uavhengig av store/små bokstaver.
    private static bool Matches(QuestionRecord record, Question question) => string.Equals(record.Text, question.Text, StringComparison.OrdinalIgnoreCase);
}
