using Quizzly.Core.Models;

namespace Quizzly.Core.Tests;

// Tester for Question: reglene spørsmålet håndhever helt på egen hånd.
// Ingen kø og ingen session her - vi tester ett objekt om gangen.
public class QuestionTests
{
    // Én fabrikkmetode i stedet for å bygge det samme spørsmålet i hver test.
    // Hver test kaller den og får sitt eget ferske objekt, så testene kan ikke påvirke hverandre.
    private static Question CreateQuestion() =>
        new(
            "Hvilken datatype bruker vi for å lagre et heltall?",
            ["int", "string", "bool", "char"],
            1,
            "Datatyper");


    // --- IsCorrect --------------------------------------------------------

    // [Fact] = én test uten parametere. Testnavnet leses som Metode_Situasjon_Forventning.
    [Fact]
    public void IsCorrect_WithCorrectUserAnswer_IsTrue()
    {
        var question = CreateQuestion();

        question.UserAnswer = 1;

        Assert.True(question.IsCorrect);
    }

    // [Theory] + [InlineData] kjører den samme testen én gang per verdi.
    // Tre gale svar, én testmetode.
    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void IsCorrect_WithWrongUserAnswer_IsFalse(int answer)
    {
        var question = CreateQuestion();

        question.UserAnswer = answer;

        Assert.False(question.IsCorrect);
    }

    // UserAnswer får standardverdien 0 når ingen har svart, og 0 er aldri et gyldig alternativ.
    [Fact]
    public void IsCorrect_BeforeAnyAnswer_IsFalse()
    {
        var question = CreateQuestion();

        Assert.Equal(0, question.UserAnswer);
        Assert.False(question.IsCorrect);
    }


    // --- CorrectAnswerText ------------------------------------------------

    // Alternativene er 1-baserte utad, men listen er 0-basert:
    // CorrectAnswer 1 skal gi det første alternativet, ikke det andre.
    [Fact]
    public void CorrectAnswerText_ReturnsTheAlternativeBehindTheNumber()
    {
        var question = CreateQuestion();

        Assert.Equal("int", question.CorrectAnswerText);
    }


    // --- IsValidAnswer ----------------------------------------------------

    // Gyldig svar er 1 til og med antall alternativer - her fire stykker.
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void IsValidAnswer_WithNumberInsideAlternatives_IsTrue(int answer)
    {
        var question = CreateQuestion();

        Assert.True(question.IsValidAnswer(answer));
    }

    // Grensene i begge ender: 0 og negative tall er for lavt, 5 og oppover for høyt.
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(5)]
    [InlineData(100)]
    public void IsValidAnswer_WithNumberOutsideAlternatives_IsFalse(int answer)
    {
        var question = CreateQuestion();

        Assert.False(question.IsValidAnswer(answer));
    }


    // --- ToString ---------------------------------------------------------

    // Assert.Collection sjekker linjene én etter én, og feiler også hvis det
    // er flere eller færre linjer enn vi har skrevet forventninger for.
    [Fact]
    public void ToString_ListsTextAndNumberedAlternatives()
    {
        var question = CreateQuestion();

        var lines = question.ToString().Split(Environment.NewLine);

        Assert.Collection(
            lines,
            first => Assert.Equal("Hvilken datatype bruker vi for å lagre et heltall?", first),
            second => Assert.Equal("1: int", second),
            third => Assert.Equal("2: string", third),
            fourth => Assert.Equal("3: bool", fourth),
            fifth => Assert.Equal("4: char", fifth));
    }

    // TrimEnd i ToString fjerner det siste linjeskiftet, så kalleren styrer luften rundt teksten selv.
    [Fact]
    public void ToString_DoesNotEndWithABlankLine()
    {
        var question = CreateQuestion();

        string text = question.ToString();

        Assert.EndsWith("4: char", text);
    }
}
