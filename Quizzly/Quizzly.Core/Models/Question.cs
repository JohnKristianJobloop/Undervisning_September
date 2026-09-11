using System.Formats.Asn1;
using System.Runtime.CompilerServices;
using System.Text;

namespace Quizzly.Core.Models;

// Primary constructor: parameterne (text, alternatives, ...) er tilgjengelige
// direkte i klassekroppen og brukes til å sette startverdier på propertyene.
// Navnene under må matche nøklene i Data/questions.json, ellers blir de ikke fylt ut.
public class Question(string text, List<string> alternatives, int correctAnswer, string category)
{
    // Properties = klassens data. { get; set; } gir lese- og skrivetilgang utenfra.
    public string Text {get; set;} = text;
    public List<string> Alternatives {get;set;} = alternatives;
    public int CorrectAnswer {get;set;} = correctAnswer;
    public int UserAnswer {get;set;}   // settes av QuizSession.SubmitAnswer (0 til å begynne med)
    public string Category {get;set;} = category;

    // Expression-bodied properties: regnes ut på nytt hver gang de leses.
    // Objektet vet selv om det er besvart riktig, vi trenger ingen bool-variabel per spørsmål.
    public bool IsCorrect => UserAnswer == CorrectAnswer;

    // Spørsmålet kjenner sine egne alternativer, så det er her regelen for gyldig svar hører hjemme.
    // Både QuizRunner (input) og QuizSession (kontroll) spør denne metoden i stedet for
    // å hardkode "1 til 4" hver for seg.
    public bool IsValidAnswer(int answer) => answer >= 1 && answer <= Alternatives.Count;
    public string CorrectAnswerText => Alternatives[CorrectAnswer - 1]; // -1 fordi lister er 0-baserte

    // ToString er arvet fra object, og override lar oss bestemme hvordan et Question
    // ser ut som tekst. Console.WriteLine(question) kaller denne helt av seg selv.
    // Vi bygger teksten i stedet for å skrive den ut: da kan modellen brukes av
    // en webapp eller en test, ikke bare av konsollen.
    public override string ToString()
    {
        // StringBuilder samler opp mange små strenger uten å lage et nytt string-objekt
        // for hver linje, slik + + + ville gjort.
        StringBuilder builder = new();
        builder.AppendLine(Text);
        // Løkken erstatter de fire hardkodede alt1-alt4-linjene fra den primitive versjonen.
        for(var i = 0; i < Alternatives.Count; i++)
        {
            builder.AppendLine($"{i + 1}: {Alternatives[i]}");
        }

        // TrimEnd fjerner den siste linjeskiftet, så kalleren styrer selv luften rundt teksten.
        return builder.ToString().TrimEnd();
    }
}