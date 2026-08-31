using System.Globalization;

// ---------------------------------------------------------------
// STEG 1: Alt bygget med primitive datatyper og enkle variabler.
// Hver del av quizen ligger i sin egen variabel: string, int, bool.
// Legg merke til hvor mye kode som må gjentas for hvert spørsmål.
// ---------------------------------------------------------------

// Tre primitive typer: tekst (string), heltall (int) og en teller vi endrer underveis.
string title = "C# Quiz";
int totalQuestions = 3;
int score = 0;

Console.WriteLine(title);
// $ foran strengen = string-interpolasjon: variabler settes rett inn i teksten.
Console.WriteLine($"Du får {totalQuestions} spørsmål. Du svarer ved å skrive inn nummeret som tilhører ditt svar.");
Console.WriteLine();

// SPØRSMÅL 1 -----------------------------------------------------
// Ett spørsmål = 6 løse variabler som ikke henger sammen på noen måte.
string question1 = "Hvilken datatype bruker vi for å lagre et heltall i C#?";
string question1alt1 = "int";
string question1alt2 = "string";
string question1alt3 = "bool";
string question1alt4 = "char";
int question1correct = 1; // Nummeret på riktig alternativ (1-basert).


// """ = raw string literal: vi skriver flere linjer uten \n og escaping.
Console.WriteLine($"""
    Spørsmål  1 av {totalQuestions}:
    {question1}
    1: {question1alt1}
    2: {question1alt2}
    3: {question1alt3}
    4: {question1alt4}
""");

// ReadLine() gir alltid tekst, og kan være null. Derfor string? (nullable).
string? userInput = Console.ReadLine();
int userAnswer;// = int.Parse(userInput);
// TryParse krasjer ikke på ugyldig input: den returnerer false og legger
// resultatet i out-variabelen. Løkken kjører til vi har et tall i riktig område.
while (!int.TryParse(userInput, out userAnswer) || userAnswer >= 5 || userAnswer < 1)
{
    Console.WriteLine("vennligst skriv et tall mellom 1 - 4...");
    userInput = Console.ReadLine();
};


// Sammenligning med == gir en bool: enten sant eller usant.
bool question1IsCorrect = question1correct == userAnswer;
if (question1IsCorrect)
{
    Console.WriteLine("Riktig!");
    score++; // ++ øker telleren med 1.
}
else
{
    Console.WriteLine($"Feil. Riktig svar var {question1alt1}");
}


// SPØRSMÅL 2 -----------------------------------------------------
// Samme oppskrift på nytt: nye variabelnavn, identisk logikk.
string question2 = "Hva er standardverdien til en bool?";
string question2alt1 = "true";
string question2alt2 = "false";
string question2alt3 = "null";
string question2alt4 = "0";
int question2correct = 2;


Console.WriteLine($"""
    Spørsmål  2 av {totalQuestions}:
    {question2}
    1: {question2alt1}
    2: {question2alt2}
    3: {question2alt3}
    4: {question2alt4}
""");

// Vi gjenbruker userInput og userAnswer, variablene er allerede deklarert over.
userInput = Console.ReadLine();
while (!int.TryParse(userInput, out userAnswer) || userAnswer >= 5 || userAnswer < 1)
{
    Console.WriteLine("vennligst skriv et tall mellom 1 - 4...");
    userInput = Console.ReadLine();
};

bool question2IsCorrect = question2correct == userAnswer;
if (question2IsCorrect)
{
    Console.WriteLine("Riktig!");
    score++;
}
else
{
    Console.WriteLine($"Feil. Riktig svar var {question2alt2}");
}


// SPØRSMÅL 3 -----------------------------------------------------
// Tredje kopi. Skal vi ha 20 spørsmål, blir dette 20 x den samme koden.
string question3 = "Hvilket nøkkelord lager en ny instans av en klasse?";
string question3alt1 = "class";
string question3alt2 = "new";
string question3alt3 = "void";
string question3alt4 = "return";
int question3correct = 2;


Console.WriteLine($"""
    Spørsmål  3 av {totalQuestions}:
    {question3}
    1: {question3alt1}
    2: {question3alt2}
    3: {question3alt3}
    4: {question3alt4}
""");

userInput = Console.ReadLine();
while (!int.TryParse(userInput, out userAnswer) || userAnswer >= 5 || userAnswer < 1)
{
    Console.WriteLine("vennligst skriv et tall mellom 1 - 4...");
    userInput = Console.ReadLine();
};

bool question3IsCorrect = question3correct == userAnswer;
if (question3IsCorrect)
{
    Console.WriteLine("Riktig!");
    score++;
}
else
{
    Console.WriteLine($"Feil. Riktig svar var {question3alt2}");
}

// ---------------------------------------------------------------
// PROBLEMET: koden kan ikke løkkes over, fordi spørsmålene bare er
// løse variabler, ikke en samling. Vi mangler også et sted å samle
// "det som hører til ett spørsmål" (tekst, alternativer, fasit, svar).
//
// Løsningen: en klasse som beskriver ETT spørsmål, og en List<T> som
// holder alle sammen. Se QuizAppClassesCollectionsAndLinq.cs.
// ---------------------------------------------------------------
