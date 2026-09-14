namespace Async.Common;


// Enkel meny for demoene: `dotnet run 2` kjører bare steg 2,
// uten argument kjøres alle stegene i rekkefølge.
public static class Menu
{
    // Hvert steg er (tittel, metode). Func<Task> gjør at vi kan await-e stegene.
    public static async Task Run(string[] args, params(string Title, Func<Task> Step)[] steps)
    {
        for (var i = 0; i < steps.Length; i++)
        {
            Console.WriteLine($"    {i + 1}. {steps[i].Title}");
        }
        // Gyldig tall som argument: kjør kun det steget.
        if (args.Length > 0 && int.TryParse(args[0], out int choice) && choice >= 1 && choice <= steps.Length)
        {
            await steps[choice - 1].Step();
            return;
        }
        // Ellers: kjør alt, ett steg om gangen slik at utskriften blir lesbar.
        foreach((_, Func<Task> step) in steps)
        {
            await step();
        }
    }
}