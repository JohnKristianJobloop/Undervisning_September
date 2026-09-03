namespace Quizzly.Core.OldModels;

// En enkel modell som viser at namespace lar oss gjenbruke klassenavn mellom filer.
// Ved å knytte en fil til et namespace blir klassens egentlige identifikator
// Quizzly.Core.OldModels.Question, og den er derfor en helt annen type enn
// Quizzly.Core.Models.Question - som er den quizen faktisk bruker.
public class Question
{
    // Til sammenligning: her er dataene offentlige felter uten oppførsel.
    // Models/Question har properties og metoder, og passer på seg selv.
    public string Title;
    public string CorrectAnswer;
}