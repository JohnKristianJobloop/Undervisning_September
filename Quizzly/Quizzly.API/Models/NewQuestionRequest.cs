using Quizzly.Core.Models;

namespace Quizzly.API.Models;


// Det klienten sender inn når et nytt spørsmål skal lages.
// Feltene er nullable fordi vi ikke kan stole på at klienten fyller ut alt – det sjekker Validate.
public sealed record NewQuestionRequest(
    string? Text,
    IReadOnlyList<string>? Alternatives,
    int CorrectAnswer,
    string? Category)
{
    private const int MinimumAlternatives = 2;

    // Samler alle feil i en ordbok (feltnavn → feilmeldinger), i formatet ValidationProblem forventer.
    // Tom ordbok betyr at forespørselen er gyldig.
    public Dictionary<string, string[]> Validate()
    {
        Dictionary<string, string[]> errors = [];
        if (string.IsNullOrWhiteSpace(Text))
        {
            errors["text"] = ["Spørsmålet må ha en tekst."];
        }
        if (string.IsNullOrWhiteSpace(Category))
        {
            errors["category"] = ["Spørsmålet må ha en kategori"];
        }
        if (Alternatives is null || Alternatives.Count < MinimumAlternatives)
        {
            errors["alternatives"] = [$"Spørsmålet må ha minst {MinimumAlternatives} alternativer."];
        }
        else if (Alternatives.Any(string.IsNullOrWhiteSpace))
        {
            errors["alternatives"] = ["Alternativene kan ikke være tomme"];
        }
        // Fasiten er 1-basert, så den må ligge mellom 1 og antall alternativer.
        else if (CorrectAnswer < 1 || CorrectAnswer > Alternatives.Count)
        {
            errors["correctAnswer"] = [$"Fasiten må peke på et alternativ (1-{Alternatives.Count})."];
        }
        return errors;
    }

    // Kalles bare etter Validate, derfor er ! (null-forgiving) trygt her. Trim fjerner mellomrom i endene.
    public Question ToQuestion() => new(Text!.Trim(), [..Alternatives!.Select(alt => alt.Trim())], CorrectAnswer, Category!.Trim());
}
