using Quizzly.Core.Models;

namespace Quizzly.Core.Tests;

// Tester for QuizSession: reglene for en gjennomkjøring - validering, rekkefølge
// og hva som skjer med et spørsmål som blir besvart feil.
// Sessionen rører verken Console eller filer, derfor kan vi teste den direkte.
public class QuizSessionTests
{
    // Standardverdier på parameterne gjør at hver test bare oppgir det den faktisk bryr seg om,
    // f.eks. CreateQuestion(correctAnswer: 2) når svaret er poenget.
    private static Question CreateQuestion(string text = "Spørsmål", int correctAnswer = 1) =>
        new(text, ["a", "b", "c", "d"], correctAnswer, "Kategori");

    // Nummererte spørsmål ("Spørsmål 1", "Spørsmål 2", ...) så vi kan kjenne igjen rekkefølgen.
    private static List<Question> CreateQuestions(int count) =>
        [.. Enumerable.Range(1, count).Select(number => CreateQuestion($"Spørsmål {number}"))];

    // --- Konstruktør ------------------------------------------------------

    [Fact]
    public void Constructor_TrimsTheTitle()
    {
        QuizSession session = new("  C# Quiz  ", CreateQuestions(1));

        Assert.Equal("C# Quiz", session.Title);
    }

    // Alle spørsmålene skal ligge i køen med en gang, før noen har svart.
    [Fact]
    public void Constructor_QueuesEveryQuestion()
    {
        QuizSession session = new("C# Quiz", CreateQuestions(3));

        Assert.Equal(3, session.RemainingCount);
        Assert.False(session.IsComplete);
    }

    // Assert.Throws fanger unntaket og gir det tilbake, så vi kan sjekke innholdet.
    // ParamName viser at unntaket peker på riktig parameter - nyttig feilmelding for kalleren.
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithBlankTitle_Throws(string title)
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(() => new QuizSession(title, CreateQuestions(1)));

        Assert.Equal("title", exception.ParamName);
    }

    // En quiz uten spørsmål gir ingen mening, så konstruktøren stopper det med en gang.
    [Fact]
    public void Constructor_WithoutQuestions_Throws()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(() => new QuizSession("C# Quiz", []));

        Assert.Equal("questions", exception.ParamName);
    }

    // null! sier til kompilatoren "jeg vet dette er null, la meg gjøre det" - det er nettopp
    // det vi vil teste: at ArgumentNullException.ThrowIfNull faktisk slår til.
    [Fact]
    public void Constructor_WithNullQuestions_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new QuizSession("C# Quiz", null!));
    }

    // Sessionen tar imot en IReviewQueue utenfra. Her sender testen inn sin egen kø
    // og ser rett inn i den - dette er gevinsten ved å programmere mot et interface.
    [Fact]
    public void Constructor_UsesTheQueueItIsGiven()
    {
        ReviewQueue<Question> queue = new();

        QuizSession session = new("C# Quiz", CreateQuestions(2), queue);

        Assert.Equal(2, queue.Count);
        Assert.Equal(queue.Count, session.RemainingCount);
    }

    // --- TryGetNextQuestion -----------------------------------------------

    // Å hente ut et spørsmål tar det ut av køen: RemainingCount går fra 2 til 1.
    [Fact]
    public void TryGetNextQuestion_WithQuestionsLeft_ReturnsTrueAndRemovesIt()
    {
        QuizSession session = new("C# Quiz", CreateQuestions(2));

        Assert.True(session.TryGetNextQuestion(out Question? question));
        Assert.NotNull(question);
        Assert.Equal(1, session.RemainingCount);
    }

    // Nye spørsmål har samme prioritet, så køen leverer dem i den rekkefølgen de ble lagt inn.
    [Fact]
    public void TryGetNextQuestion_HandsOutQuestionsInOrder()
    {
        QuizSession session = new("C# Quiz", CreateQuestions(3));

        session.TryGetNextQuestion(out Question? first);
        session.TryGetNextQuestion(out Question? second);

        Assert.Equal("Spørsmål 1", first?.Text);
        Assert.Equal("Spørsmål 2", second?.Text);
    }

    // Try-mønsteret: tom kø er ikke en feil, men et "nei" - false og null i stedet for unntak.
    // Det er dette som gjør while (session.TryGetNextQuestion(out var q)) mulig i konsoll-laget.
    [Fact]
    public void TryGetNextQuestion_OnEmptySession_ReturnsFalse()
    {
        QuizSession session = new("C# Quiz", CreateQuestions(1));

        session.TryGetNextQuestion(out Question? _);

        Assert.False(session.TryGetNextQuestion(out Question? question));
        Assert.Null(question);
        Assert.True(session.IsComplete);
    }

    // --- SubmitAnswer, riktig svar ----------------------------------------

    [Fact]
    public void SubmitAnswer_WithCorrectAnswer_ReturnsTrue()
    {
        QuizSession session = new("C# Quiz", [CreateQuestion(correctAnswer: 2)]);
        session.TryGetNextQuestion(out Question? question);

        Assert.True(session.SubmitAnswer(question!, 2));
    }

    // Riktig svar = ferdig med spørsmålet. Køen er tom, og quizen er over.
    [Fact]
    public void SubmitAnswer_WithCorrectAnswer_DoesNotQueueTheQuestionAgain()
    {
        QuizSession session = new("C# Quiz", [CreateQuestion(correctAnswer: 2)]);
        session.TryGetNextQuestion(out Question? question);

        session.SubmitAnswer(question!, 2);

        Assert.True(session.IsComplete);
    }

    // Svaret lagres PÅ spørsmålet, ikke bare returneres. Da kan vi lage statistikk etterpå.
    [Fact]
    public void SubmitAnswer_RecordsTheAnswerOnTheQuestion()
    {
        QuizSession session = new("C# Quiz", [CreateQuestion(correctAnswer: 2)]);
        session.TryGetNextQuestion(out Question? question);

        session.SubmitAnswer(question!, 3);

        Assert.Equal(3, question!.UserAnswer);
    }

    // --- SubmitAnswer, feil svar ------------------------------------------

    [Fact]
    public void SubmitAnswer_WithWrongAnswer_ReturnsFalse()
    {
        QuizSession session = new("C# Quiz", [CreateQuestion(correctAnswer: 2)]);
        session.TryGetNextQuestion(out Question? question);

        Assert.False(session.SubmitAnswer(question!, 4));
    }

    // Feil svar legger spørsmålet tilbake i køen: quizen er ikke ferdig før alt er riktig.
    [Fact]
    public void SubmitAnswer_WithWrongAnswer_QueuesTheQuestionAgain()
    {
        QuizSession session = new("C# Quiz", [CreateQuestion(correctAnswer: 2)]);
        session.TryGetNextQuestion(out Question? question);

        session.SubmitAnswer(question!, 4);

        Assert.False(session.IsComplete);
        Assert.Equal(1, session.RemainingCount);
    }

    // Kjerneregelen i hele appen: et feilbesvart spørsmål får ReviewPriority (5) og havner
    // bak spørsmålene som ennå ikke er stilt (1). Løkken tømmer køen og noterer rekkefølgen,
    // og svarer riktig hver gang så vi ikke går i evig løkke.
    [Fact]
    public void SubmitAnswer_WithWrongAnswer_PutsTheQuestionBehindTheNewOnes()
    {
        List<Question> questions =
        [
            CreateQuestion("Spørsmål 1"),
            CreateQuestion("Spørsmål 2"),
            CreateQuestion("Spørsmål 3")
        ];

        QuizSession session = new("C# Quiz", questions);
        session.TryGetNextQuestion(out Question? first);
        session.SubmitAnswer(first!, 4);

        List<string> order = [];

        while (session.TryGetNextQuestion(out Question? question))
        {
            order.Add(question.Text);
            session.SubmitAnswer(question, question.CorrectAnswer);
        }

        Assert.Equal(["Spørsmål 2", "Spørsmål 3", "Spørsmål 1"], order);
    }

    // Assert.Same sjekker at det er det SAMME objektet som kommer tilbake, ikke en kopi.
    // Derfor husker spørsmålet fortsatt det gale forsøket når vi får det på nytt.
    [Fact]
    public void SubmitAnswer_WithCorrectAnswerOnSecondTry_EndsTheSession()
    {
        QuizSession session = new("C# Quiz", [CreateQuestion(correctAnswer: 2)]);

        session.TryGetNextQuestion(out Question? first);
        session.SubmitAnswer(first!, 4);

        session.TryGetNextQuestion(out Question? again);
        session.SubmitAnswer(again!, 2);

        Assert.True(session.IsComplete);
        Assert.Same(first, again);
    }

    // --- SubmitAnswer, ugyldige argumenter --------------------------------

    // Et svar utenfor alternativene er en programmeringsfeil, ikke en brukerfeil:
    // konsoll-laget skal ha validert input før det kommer hit.
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(5)]
    public void SubmitAnswer_WithAnswerOutsideAlternatives_Throws(int answer)
    {
        QuizSession session = new("C# Quiz", [CreateQuestion()]);
        session.TryGetNextQuestion(out Question? question);

        ArgumentOutOfRangeException exception =
            Assert.Throws<ArgumentOutOfRangeException>(() => session.SubmitAnswer(question!, answer));

        Assert.Equal("answer", exception.ParamName);
    }

    [Fact]
    public void SubmitAnswer_WithNullQuestion_Throws()
    {
        QuizSession session = new("C# Quiz", CreateQuestions(1));

        Assert.Throws<ArgumentNullException>(() => session.SubmitAnswer(null!, 1));
    }
}
