// ---------------------------------------------------------------
// STEG 2: Samme quiz, men nå med klasser, samlinger og LINQ.
// Data og oppførsel som hører sammen ligger i samme objekt,
// og alle spørsmålene ligger i en liste vi kan løkke over.
// ---------------------------------------------------------------

// List<Question> = en samling som kan vokse og krympe (i motsetning til string[]).
// [ ... ] er collection expression - kortform for å fylle lista med en gang.
List<Question> questions = 
[
    // Hvert kall til new lager ett Question-objekt. Alt som hørte til
    // "spørsmål 1" i den primitive versjonen er nå samlet på ett sted.
    new Question(
        "Hvilken datatype bruker vi for å lagre et heltall?",
        ["int", "string", "bool", "char"], // alternativene er sin egen liste
        1,                                  // nummeret på riktig svar
        "Datatyper"),                       // kategori - brukes til statistikk til slutt

    new Question(
        "Hva er standardverdien til en bool?",
        ["true", "false", "null", "0"],
        2,
        "Datatyper"),

    new Question(
        "Hvilket nøkkelord lager en ny instans av en klasse?",
        ["class", "new", "void", "return"],
        2,
        "Klasser"),

    new Question(
        "Hva kaller vi en metode som kjører når objektet opprettes?",
        ["Destruktor", "Property", "Konstruktør", "Interface"],
        3,
        "Klasser"),

    new Question(
        "Hvilken samling kan vokse og krympe under kjøring?",
        ["string[]", "List<T>", "int", "char"],
        2,
        "Samlinger"),

    new Question(
        "Hvilken LINQ-metode filtrerer ut elementer som matcher et vilkår?",
        ["Select", "Where", "OrderBy", "Count"],
        2,
        "LINQ")
];

// Hele programmet er nå tre linjer: lag quizen, kjør den, vis resultatet.
// Vil du ha 20 spørsmål? Legg dem i listen over, resten av koden er uendret.
var quiz = new Quiz("C# Quiz", questions);
quiz.Run();
quiz.PrintSummary();




// ---------------------------------------------------------------
// KLASSENE
// ---------------------------------------------------------------

// Primary constructor: parameterne (text, alternatives, ...) er tilgjengelige
// direkte i klassekroppen og brukes til å sette startverdier på propertyene.
class Question(string text, List<string> alternatives, int correctAnswer, string category)
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

// Quiz eier hele listen med spørsmål og styrer gjennomkjøringen.
class Quiz(string title, List<Question> questions)
{
    public string Title{get;set;} = title;
    public List<Question> Questions {get;set;} = questions;


    // Kjører gjennom alle spørsmålene.
    public void Run()
    {
        Console.WriteLine(Title);
        Console.WriteLine($"Du får {Questions.Count} spørsmål");
        Console.WriteLine();

        int questionNumber = 1;

        // foreach: hele den kopierte blokken fra steg 1 kjøres nå en gang per objekt i listen.
        foreach (var question in Questions)
        {
            Console.WriteLine($"Spørsmål {questionNumber} av {Questions.Count}");
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
                Console.WriteLine($"Feil. Riktig svar var {question.CorrectAnswerText}");
            }
            Console.WriteLine();
            questionNumber++;
        }
    }

    // ---------------------------------------------------------------
    // LINQ: fordi svarene ligger i en samling, kan vi spørre om dem etterpå.
    // Dette var praktisk talt umulig med løse variabler.
    // ---------------------------------------------------------------
    public void PrintSummary()
    {
        // Count med vilkår teller alle riktige. Erstatter score++ underveis.
        var score = Questions.Count(question => question.IsCorrect);
        // (double) tvinger flyttallsdivisjon; uten den ville int/int gitt 0.
        double percentage = (double)score/Questions.Count*100;

        Console.WriteLine("----RESULTAT!-----");
        Console.WriteLine($"Du fikk {score} av {Questions.Count} riktige ({percentage:0.0} %)."); // :0.0 = én desimal
        Console.WriteLine();

        // Where filtrerer. [..spread] gjør resultatet om til en ny List<Question>.
        List<Question> wrongAnswers = [..Questions.Where(question => !question.IsCorrect)];

        if (wrongAnswers.Any()) // Any() = "finnes det noen i det hele tatt?"
        {
            Console.WriteLine("Disse bør du repetere:");

            // Select plukker ut en property fra hvert objekt, her bare spørsmålsteksten.
            foreach (var text in wrongAnswers.Select(question => question.Text))
            {
                Console.WriteLine($" - {text}");
            }
            Console.WriteLine();
        }

        var resultPerCategory = Questions
                // GroupBy samler spørsmålene i grupper per kategori.
                .GroupBy(question => question.Category)
                // Select bygger et anonymt objekt med tallene vi vil vise.
                .Select(group => 
                new
                {
                    Category = group.Key,  // verdien det ble gruppert på
                    Correct = group.Count(question => question.IsCorrect),
                    Total = group.Count()
                })
                // Sorter på flest riktige først, deretter alfabetisk på kategori.
                .OrderByDescending(result => result.Correct)
                .ThenBy(result => result.Category);

        Console.WriteLine("Poeng per kategori:");
        foreach (var result in resultPerCategory)
        {
            Console.WriteLine($" - {result.Category}: {result.Correct} / {result.Total}");
        }
    }
}
