using Quizzly.Core.Models;

namespace Quizzly.API.Data;

// Lagringsformatet: hvordan ett spørsmål ser ut i questions.json.
// Vi deserialiserer til denne i stedet for rett til Question, så filformatet og
// domenemodellen i Core kan endre seg uavhengig av hverandre.
public sealed record QuestionRecord(
    string Text,
    IReadOnlyList<string> Alternatives,
    int CorrectAnswer,
    string Category
)
{
    // Domenemodell -> lagringsformat (brukes når vi skal skrive tilbake til fil).
    public static QuestionRecord From(Question question) => new(question.Text, question.Alternatives, question.CorrectAnswer, question.Category);

    // Lagringsformat -> domenemodell. [..Alternatives] kopierer til en ny List<string>,
    // som er det Question krever.
    public Question ToQuestion() => new(Text, [..Alternatives], CorrectAnswer, Category);
}
