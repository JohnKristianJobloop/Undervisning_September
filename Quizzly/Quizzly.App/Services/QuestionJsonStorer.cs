using System.Text.Json;
using Quizzly.Core.Models;

namespace Quizzly.App.Services;

// Statisk hjelpeklasse (ingen tilstand, ingen new): den bare leser spørsmål fra fil.
public static class QuestionJsonStorer
{
    // Leser filen som tekst og gjør JSON-en om til Question-objekter.
    // Returtypen List<Question>? har ?, fordi Deserialize kan gi null.
    public static List<Question>? LoadQuestionsFromFile(string filepath)
    {
        if (!File.Exists(filepath))
        {
            throw new ArgumentException($"{filepath} not a file");
        }
        var jsonStringInput = File.ReadAllText(filepath);
        // Navnene i JSON-filen må matche propertynavnene i Question.
        List<Question>? questions = JsonSerializer.Deserialize<List<Question>>(jsonStringInput);
        return questions;
    }    
}