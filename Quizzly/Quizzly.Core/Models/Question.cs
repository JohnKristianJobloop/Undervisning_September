namespace Quizzly.Core.Models;

// Primary constructor: parameterne (text, alternatives, ...) er tilgjengelige
// direkte i klassekroppen og brukes til å sette startverdier på propertyene.
public class Question(string text, List<string> alternatives, int correctAnswer, string category)
{
    // Properties = klassens data. { get; set; } gir lese- og skrivetilgang utenfra.
    public string Text {get; set;} = text;
    public List<string> Alternatives {get;set;} = alternatives;
    public int CorrectAnswer {get;set;} = correctAnswer;
    public int UserAnswer {get;set;}   // fylles inn mens quizen kjører (0 til å begynne med)
    public string Category {get;set;} = category;

    // Expression-bodied properties: regnes ut på nytt hver gang de leses.
    // Objektet vet selv om det er besvart riktig, vi trenger ingen bool-variabel per spørsmål.
    public bool IsCorrect => UserAnswer == CorrectAnswer;
    public string CorrectAnswerText => Alternatives[CorrectAnswer - 1]; // -1 fordi lister er 0-baserte

    // Oppførsel i samme klasse som dataene: spørsmålet kan skrive ut seg selv.
    public void PrintQuestion()
    {
        Console.WriteLine(Text);
        // Løkken erstatter de fire hardkodede alt1-alt4-linjene fra den primitive versjonen.
        for (var i = 0; i < Alternatives.Count; i++)
        {
            Console.WriteLine($"{i+1}: {Alternatives[i]}");
        }
    }
}