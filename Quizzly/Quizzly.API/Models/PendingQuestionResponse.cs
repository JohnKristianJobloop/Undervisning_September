using Quizzly.Core.Models;

namespace Quizzly.API.Models;

// Spørsmålet slik brukeren skal se det: teksten og alternativene, men UTEN fasit.
// Det er hele poenget med egne responsmodeller - vi velger selv hva som sendes over nett.
public sealed record PendingQuestionResponse(
    string Text,
    List<string> Alternatives,
    string Category
)
{
    // Oversetter fra domenemodell til respons. CorrectAnswer blir bevisst utelatt.
    public static PendingQuestionResponse From(Question question) => new(question.Text, question.Alternatives, question.Category);
}
