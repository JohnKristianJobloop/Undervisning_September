using Quizzly.Core.Models;

namespace Quizzly.App.Models;


// Quiz eier hele listen med spørsmål og styrer gjennomkjøringen.
public class Quiz
{
    // Prioritet i køen: lavt tall først. Nye spørsmål stilles altså før repetisjonene.
    private const int NewPriority = 1;
    private const int ReviewPriority = 5;
    public string Title{get;set;}
    public ReviewQueue<Question> Questions {get;set;}

    // Konstruktøren tar en vanlig liste og fyller den over i køen.
    public Quiz(string title, List<Question> questions)
    {
        Title = title;
        Questions = new();

        foreach(var question in questions)
        {
            Questions.Enqueue(question, NewPriority);
        }
    }
    // Kjører hele quizen. foreach tømmer køen, så løkken varer til alt er besvart riktig.
    public void Run()
    {
        Console.WriteLine(Title);
        Console.WriteLine($"Du får {Questions.Count} spørsmål. Svarer du feil, blir spørsmålet stilt igjen.");
        Console.WriteLine();

        foreach (var question in Questions)
        {
            Console.WriteLine($"[{question.Category}] {Questions.Count}");
            question.PrintQuestion();
            Console.WriteLine("Ditt Svar: ");
            string? input = Console.ReadLine();
            int answer;
            // Samme validering som før, men skrevet ett sted i stedet for tre.
            while(!int.TryParse(input, out answer) || answer >= 5 || answer < 1)
            {
                Console.WriteLine("Vennligst srkiv et tall mellom 1 - 4");
                input = Console.ReadLine();
            }

            // Vi lagrer svaret PÅ objektet, så det finnes fortsatt når vi lager statistikk til slutt.
            question.UserAnswer = answer;

            if (question.IsCorrect)
            {
                Console.WriteLine("Riktig!");
            }
            else
            {
                // Objektet slår selv opp riktig svartekst.
                Console.WriteLine($"Feil. Du får mulighet å prøve igjen senere.");
                Questions.Enqueue(question, ReviewPriority);
            }
            Console.WriteLine();
        }
    }
}
