namespace Quizzly.API.Data;

// Options-mønsteret: innstillinger samlet i en egen klasse i stedet for hardkodet i koden.
// Verdiene kan overstyres fra appsettings.json under seksjonen "QuestionStore".
public sealed class QuestionStoreOptions
{
    // Navnet på seksjonen i appsettings.json, som konstant så vi ikke skriver strengen flere steder.
    public const string SectionName = "QuestionStore";
    // Standardverdi: brukes når ingenting er satt i konfigurasjonen. Stien er relativ til prosjektmappen.
    public string FilePath{get;set;} = "Data/questions.json";
}
