using Quizzly.App.Models;
using Quizzly.App.Services;

/* var queue = new ReviewQueue<int>();
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
var questions = QuestionJsonStorer.LoadQuestionsFromFile("Data/questions.json") ?? throw new NullReferenceException("Missing questions");
// 2) Bygg quizen av spørsmålene og kjør den.
var quiz = new Quiz("C# quiz", questions);
quiz.Run();
