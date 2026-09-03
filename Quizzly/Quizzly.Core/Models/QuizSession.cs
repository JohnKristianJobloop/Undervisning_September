using System.Diagnostics.CodeAnalysis;
using Quizzly.Core.Interfaces;

namespace Quizzly.Core.Models;

// QuizSession forklarer reglene for gjennomkjøring av en quiz, med kø, regler og validering.
// Klassen ligger i Core og bruker verken Console eller filer, derfor kan den gjenbrukes
// av en konsollapp, en webapp eller en enhetstest uten endringer.
public class QuizSession
{
    // Prioritet i køen: lavt tall først. Nye spørsmål stilles altså før repetisjonene.
    private const int NewPriority = 1;
    private const int ReviewPriority = 5;
    public string Title{get;set;}
    // Feltet er typet som interfacet, ikke som ReviewQueue<Question>. Sessionen bryr seg
    // om HVA køen kan gjøre, ikke HVORDAN den gjør det.
    private IReviewQueue<Question> _questions {get;set;}
    // Utledede properties: regnes ut fra køen hver gang de leses, så de kan aldri bli utdaterte.
    public int RemainingCount => _questions.Count;
    public bool IsComplete => _questions.IsEmpty;

    // Siste parameter har standardverdien null, så vanlig bruk er new QuizSession(tittel, spørsmål).
    // Tester (eller en annen kø-implementasjon) kan likevel sende inn sin egen kø.
    public QuizSession(string title, IEnumerable<Question> questions, IReviewQueue<Question>? queue = null)
    {
       Title = ValidateTitle(title);
       // ?? gir venstre side hvis den ikke er null, ellers høyre: her en helt vanlig ReviewQueue.
       _questions = queue ?? new ReviewQueue<Question>();
       // Alle spørsmål legges inn som "nye".
       foreach (var question in ValidateQuestions(questions))
        {
            _questions.Enqueue(question, NewPriority);
        }
    }

    // Try-mønsteret igjen: false betyr tom kø. [MaybeNullWhen(false)] forteller kompilatoren
    // at question bare er null når vi returnerer false, så kalleren slipper null-advarsler.
    public bool TryGetNextQuestion([MaybeNullWhen(false)] out Question question)
    {
        return _questions.TryDequeue(out question);
    }

    // Hjertet i quizen: tar imot et svar, avgjør om det er riktig, og bestemmer om
    // spørsmålet skal tilbake i køen. Kalleren trenger bare bry seg om true/false.
    public bool SubmitAnswer(Question question, int answer)
    {
        ArgumentNullException.ThrowIfNull(question);

        // Ugyldige svar er en programmeringsfeil her - konsoll-laget har allerede validert input.
        if (!question.IsValidAnswer(answer))
        {
            throw new ArgumentOutOfRangeException(
                nameof(answer),
                $"Svaret må være et tall mellom 1 og {question.Alternatives.Count}."
            );
        }

        // Svaret lagres PÅ objektet, så det finnes fortsatt hvis vi vil lage statistikk til slutt.
        question.UserAnswer = answer;
        if (!question.IsCorrect)
        {
            // Feil svar legges bakerst med høyere tall, så det dukker opp igjen etter de nye.
            _questions.Enqueue(question, ReviewPriority);
        }
        return question.IsCorrect;
    }

    // Guard-metodene under kjører før objektet er ferdig bygget, derfor er de static:
    // de bruker bare parameterne sine, ingen tilstand på instansen.
    private static string ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Quiz needs a valid title.", nameof(title));
        }
        return title.Trim();
    }

    private static List<Question> ValidateQuestions(IEnumerable<Question> questions)
    {
        ArgumentNullException.ThrowIfNull(questions);
        // [..questions] er en collection expression: den kopierer sekvensen til en ny liste.
        // Kopien gjør at vi kan telle elementene, og at endringer utenfra ikke påvirker oss.
        List<Question> validated = [..questions];
        if (validated.Count == 0)
        {
            throw new ArgumentException("Quiz needs atleast one question.", nameof(questions));
        }
        return validated;
    }

}