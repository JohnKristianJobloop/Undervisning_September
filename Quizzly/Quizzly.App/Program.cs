using Quizzly.App.Models;
using Quizzly.App.Services;
using Quizzly.Core.Models;

/* Demoen fra dag 2: ReviewQueue<T> fungerer like godt med int som med Question.

var queue = new ReviewQueue<int>();
queue.Enqueue(10, 1);
queue.Enqueue(5, 9);
queue.Enqueue(12, 2);
queue.Enqueue(42, 1);
queue.Enqueue(4, 9);

foreach (var item in queue)
{
    Console.WriteLine(item);
} */

// 1) Hent spørsmålene fra JSON-filen som kopieres til output-mappen ved bygg.
// Deserialisering kan gi null (tom eller ugyldig fil), så vi stopper med en gang.
var questions = await QuestionJsonStorer.LoadQuestionsFromFile("Data/questions.json") ?? throw new NullReferenceException("Missing questions");
// 2) QuizSession (Core) eier reglene og tilstanden: køen, rekkefølgen og hva som er riktig.
// Den kjenner ikke til konsollen i det hele tatt.
var session = new QuizSession("C# quiz", questions);
// 3) QuizRunner (App) er presentasjonslaget: den leser input og skriver ut svar.
// Skillet gjør at vi kan bytte ut konsollen med web eller tester uten å røre reglene.
var quiz = new QuizRunner(session);
quiz.Run();
