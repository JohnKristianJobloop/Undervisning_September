using Quizzly.Core.Models;

namespace Quizzly.API.Models;

// Hele spørsmålet, fasit inkludert. Brukes der svaret allerede er avgitt,
// eller av administrative endepunkter - ikke når brukeren skal svare.
public sealed record QuestionResponse(
    string Text,
    IReadOnlyList<string> Alternatives,
    int CorrectAnswer,
    string Category
)
{
    // Statisk fabrikkmetode: ett sted som vet hvordan et Question blir til en respons.
    public static QuestionResponse From(Question question) => new(question.Text, question.Alternatives, question.CorrectAnswer, question.Category);
}
