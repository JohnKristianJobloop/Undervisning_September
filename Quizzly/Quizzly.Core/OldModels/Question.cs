namespace Quizzly.Core.OldModels;

// En simpel modell for å vise at namespace lar oss dele klassenavn mellom filer. 
// ved å knytte en fil til et namespace blir klassen's egentlige idenfitikator:
// Quizzly.Core.OldModels.Question, og er derfor markant forskjellig fra
// Quizzly.Core.Models.Question
public class Question
{
    public string Title;
    public string CorrectAnswer;
}