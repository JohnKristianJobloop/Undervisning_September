using Quizzly.Core.Models;

namespace Quizzly.App.Models;


// QuizRunner er konsoll-laget rundt quizen: den leser tastetrykk og skriver til skjermen.
// Alle reglene (hvilket spørsmål er neste, var svaret riktig, skal det repeteres)
// ligger i QuizSession. Denne klassen spør bare, og viser fram resultatet.
public class QuizRunner
{
    // Startverdi for out-parametere før vi har lest et gyldig svar.
    // 0 er trygt, fordi alternativene alltid nummereres fra 1.
    private const int NoAnswer = 0;
    // readonly: sessionen settes en gang i konstruktøren og kan aldri byttes ut etterpå.
    private readonly QuizSession _session;

    // Vi lager ikke sessionen selv, vi får den inn utenfra (dette heter dependency injection).
    // Da bestemmer Program.cs hvilken quiz som kjøres, ikke runneren.
    public QuizRunner(QuizSession session)
    {
        // Feil tidlig og tydelig: uten session har runneren ingenting å kjøre.
        ArgumentNullException.ThrowIfNull(session);
        _session = session;
    }

    public void Run()
    {
        PrintIntro();
        // Try-mønsteret: metoden returnerer false når køen er tom, og løkken stopper.
        // "out Question? question" deklarerer variabelen rett i kallet.
        while(_session.TryGetNextQuestion(out Question? question))
        {
            PrintQuestion(question);
            // Brukeren kan avbryte (Ctrl+D / lukket input). Da er det ingen vits å spørre videre.
            if(!TryReadAnswer(question, out int answer))
            {
                Console.WriteLine("Avslutter Quiz");
                break;
            }
            // Sessionen avgjør om svaret var riktig og legger feilsvar tilbake i køen.
            PrintResult(question, _session.SubmitAnswer(question, answer));
            Console.WriteLine();
        }
    }

    private void PrintIntro()
    {
        Console.WriteLine(_session.Title);
        Console.WriteLine($"Du får {_session.RemainingCount} spørsmål.");
        Console.WriteLine("Svarer du feil, kommer spørsmålet på nytt senere.\n");
    }

    private void PrintQuestion(Question question)
    {
        PrintQuestionHeader(question);
        // WriteLine kaller question.ToString() automatisk - spørsmålet formaterer seg selv.
        Console.WriteLine(question);
    }

    private void PrintQuestionHeader(Question question)
    {
        Console.WriteLine($"[{question.Category}] {_session.RemainingCount} igjen i køen");
    }

    // static fordi metoden ikke rører _session: alt den trenger kommer inn som parameter.
    // Returnerer false hvis brukeren avslutter, true når vi har et gyldig svar i out-parameteren.
    private static bool TryReadAnswer(Question question, out int answer)
    {
        answer = NoAnswer;

        // Løkken går til brukeren enten skriver noe gyldig eller avbryter.
        while (true)
        {
            Console.WriteLine("Ditt svar: ");
            var input = Console.ReadLine();
            // null betyr slutt på input-strømmen, ikke tom linje.
            if (input is null)
            {
                Console.WriteLine();
                return false;
            }
            if (TryValidateAnswer(input, question, out answer))
            {
                return true;
            }
            // Antall alternativer hentes fra spørsmålet, så teksten stemmer selv om
            // et spørsmål har flere eller færre enn fire valg.
            Console.WriteLine($"Ugyldig svar. Skriv et tall mellom 1 og {question.Alternatives.Count}");
        }
    }

    // To ting kan gå galt: teksten er ikke et tall, eller tallet peker utenfor alternativene.
    private static bool TryValidateAnswer(string? input, Question question, out int answer)
    {
        answer = NoAnswer;

        if (!int.TryParse(input?.Trim(), out answer))
        {
            return false;
        }

        // Spørsmålet kjenner sine egne alternativer, så det avgjør selv hva som er lovlig.
        if (!question.IsValidAnswer(answer))
        {
            return false;
        }
        return true;
    }

    // Tar imot resultatet fra sessionen. Runneren regner ikke ut noe selv, den skriver bare ut.
    private static void PrintResult(Question question, bool isCorrect)
    {
        if (isCorrect)
        {
            Console.WriteLine("Riktig!");
            return;
        }
        Console.WriteLine("Feil svar, prøv igjen senere!");
    }
}
