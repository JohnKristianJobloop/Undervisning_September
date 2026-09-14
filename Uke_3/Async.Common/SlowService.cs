namespace Async.Common;


// Falsk tjeneste som later som den er treg, slik at vi ser ventingen i demoene.
// Poenget er kontrasten mellom de to metodene under.
public static class SlowService
{
    // Synkron: Thread.Sleep holder på tråden mens vi venter. Tråden gjør ingenting,
    // men ingen andre får bruke den heller.
    public static string Fetch(string name, int ms = 500)
    {
        Thread.Sleep(500);
        return $"svar fra {name}";
    }

    // Asynkron: Task.Delay gir tråden tilbake mens vi venter, og tar en ny tråd
    // når svaret er klart. token gjør at venting kan avbrytes.
    public static async Task<string> FetchAsync(string name, int ms = 500, CancellationToken token = default)
    {
        await Task.Delay(ms, token);
        return $"svar fra {name}";
    }
}