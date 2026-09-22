namespace Quizzly.API.Models;

// Contracts = formen på dataene som går inn og ut av API-et.
// De er egne typer nettopp fordi de ikke skal tvinges til å se ut som domenemodellen.

// Det klienten sender inn når den svarer på et spørsmål.
public sealed record AnswerRequest(int Answer);

// Det klienten får tilbake: fasit og hvor mange spørsmål som gjenstår.
public sealed record AnswerResponse(
    string Text,
    bool Correct, 
    int CorrectAnswer,
    string CorrectAnswerText,
    int Remaining
);

// Utfallet av et svarforsøk. Enum i stedet for bool, fordi det finnes flere svar enn ja/nei:
// endepunktet kan bruke dette til å velge riktig HTTP-statuskode.
public enum AnswerStatus
{
    Answered,
    QueueEmpty,
    InvalidAnswer
}

// Samler status og data i ett returobjekt. response er null når det ikke ble noe svar å gi,
// og AlternativesCount brukes i feilmeldingen ved ugyldig svar ("velg 1 til 4").
public sealed record AnswerResult(AnswerStatus status, AnswerResponse? response = null, int AlternativesCount = 0);
