using Quizzly.Core.Models;

namespace Quizzly.API.Models;


// Slik et spørsmål ser ut i databasen (én rad i tabellen Questions).
// Holdes adskilt fra domenemodellen Question, så databasen ikke styrer resten av koden.
public sealed class QuestionEntity
{
    // Primærnøkkel – databasen gir hver rad et nytt Id automatisk.
    public int Id {get;set;}
    public required string Text {get;set;}
    // Teksten i store bokstaver. Brukes til å finne duplikater uavhengig av store/små bokstaver.
    public required string NormalizedText {get;set;}
    public required List<string> Alternatives {get;set;}
    public int CorrectAnswer {get;set;}
    public required string Category {get;set;}
    public static string Normalize(string text) => text.ToUpperInvariant();
    // Domenemodell → databasemodell.
    public static QuestionEntity From(Question question) => new()
    {
        Text = question.Text,
        NormalizedText = Normalize(question.Text),
        Alternatives = [..question.Alternatives],
        CorrectAnswer = question.CorrectAnswer,
        Category = question.Category
    };
    // Databasemodell → domenemodell.
    public Question ToQuestion() => new(Text, Alternatives, CorrectAnswer, Category);
}
