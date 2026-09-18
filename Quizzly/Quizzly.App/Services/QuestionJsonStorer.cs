using System.Text.Json;
using Quizzly.Core.Models;

namespace Quizzly.App.Services;

// Statisk hjelpeklasse (ingen tilstand, ingen new): den bare leser spørsmål fra fil.
// Innlesing er en jobb for App-laget - Core skal ikke vite om filer eller JSON.
public static class QuestionJsonStorer
{
    // Leser filen som tekst og gjør JSON-en om til Question-objekter fra Quizzly.Core.
    // Returtypen List<Question>? har ?, fordi Deserialize kan gi null.
    public static async Task<List<Question>?> LoadQuestionsFromFile(string filepath)
    {
        // Stien er relativ til output-mappen: csproj-en kopierer Data/questions.json dit ved bygg.
        if (!File.Exists(filepath))
        {
            throw new ArgumentException($"{filepath} not a file");
        }
        using var fileStream = File.OpenRead(filepath);
        // Navnene i JSON-filen må matche propertynavnene i Question.
        List<Question>? questions = await JsonSerializer.DeserializeAsync<List<Question>>(fileStream);
        return questions;
    }    
}