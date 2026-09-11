using Quizzly.Core.Models;

namespace Quizzly.Core.Tests;


// Tester for den generiske køen. Vi bruker string som T, ikke Question:
// da ser vi rekkefølgen rett i assertene, og testene sier noe om køen alene.
public class ReviewQueueTests
{
    // De samme to prioritetene som QuizSession bruker. Lavt tall = tidlig i køen.
    private const int NewPriority = 1;
    private const int ReviewPriority = 5;

    // --- Tom kø -----------------------------------------------------------

    // [] er en collection expression, og betyr her bare "en ny, tom kø".
    [Fact]
    public void NewQueue_IsEmpty()
    {
        ReviewQueue<string> queue = [];

        Assert.True(queue.IsEmpty);
        Assert.Equal(0, queue.Count);
    }

    // Dequeue på tom kø er en feil - da har kalleren brukt klassen galt.
    // Vi sjekker også meldingen, så teksten utvikleren får se ikke endrer seg ved et uhell.
    [Fact]
    public void Dequeue_OnEmptyQueue_Throws()
    {
        ReviewQueue<string> queue = [];

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => queue.Dequeue());

        Assert.Equal("No elements in Queue", exception.Message);
    }

    [Fact]
    public void Peek_OnEmptyQueue_Throws()
    {
        ReviewQueue<string> queue = [];

        Assert.Throws<InvalidOperationException>(() => queue.Peek());
    }

    // Try-varianten kaster ikke: tom kø gir false og default-verdien (null for string).
    [Fact]
    public void TryDequeue_OnEmptyQueue_ReturnsFalse()
    {
        ReviewQueue<string> queue = [];

        Assert.False(queue.TryDequeue(out string? item));
        Assert.Null(item);
    }

    // --- Enqueue og Dequeue -----------------------------------------------

    [Fact]
    public void Enqueue_IncreasesCount()
    {
        ReviewQueue<string> queue = [];

        queue.Enqueue("a", NewPriority);
        queue.Enqueue("b", NewPriority);

        Assert.Equal(2, queue.Count);
        Assert.False(queue.IsEmpty);
    }

    // Dequeue både henter ut OG fjerner - derfor er køen tom etterpå.
    [Fact]
    public void Dequeue_RemovesTheItemItReturns()
    {
        ReviewQueue<string> queue = [];
        queue.Enqueue("a", NewPriority);

        Assert.Equal("a", queue.Dequeue());
        Assert.True(queue.IsEmpty);
    }

    [Fact]
    public void TryDequeue_WithItems_ReturnsTrueAndTheItem()
    {
        ReviewQueue<string> queue = [];
        queue.Enqueue("a", NewPriority);

        Assert.True(queue.TryDequeue(out string? item));
        Assert.Equal("a", item);
    }

    // Forskjellen på Peek og Dequeue: Peek kikker bare, elementet blir liggende.
    [Fact]
    public void Peek_ReturnsFirstItemWithoutRemovingIt()
    {
        ReviewQueue<string> queue = [];
        queue.Enqueue("a", NewPriority);

        Assert.Equal("a", queue.Peek());
        Assert.Equal(1, queue.Count);
    }


    // --- Prioritet --------------------------------------------------------

    // Lagt inn sist, men med lavest tall: "ny" skal likevel ut først.
    [Fact]
    public void Enqueue_PutsLowerPriorityNumberFirst()
    {
        ReviewQueue<string> queue = [];

        queue.Enqueue("repetisjon", ReviewPriority);
        queue.Enqueue("ny", NewPriority);

        Assert.Equal("ny", queue.Dequeue());
        Assert.Equal("repetisjon", queue.Dequeue());
    }

    // Lik prioritet skal ikke stokke om: den som kom først, kommer først ut.
    // ToList() går gjennom køen, og siden det tømmer den, er vi kun ute etter rekkefølgen her.
    [Fact]
    public void Enqueue_WithEqualPriority_KeepsInsertionOrder()
    {
        ReviewQueue<string> queue = [];

        queue.Enqueue("a", NewPriority);
        queue.Enqueue("b", NewPriority);
        queue.Enqueue("c", NewPriority);

        Assert.Equal(["a", "b", "c"], queue.ToList());
    }

    // Begge reglene samtidig: først sortert på prioritet, og innenfor lik prioritet
    // på rekkefølgen de ble lagt inn.
    [Fact]
    public void Enqueue_WithMixedPriorities_OrdersByPriorityThenInsertion()
    {
        ReviewQueue<string> queue = [];

        queue.Enqueue("sist", 9);
        queue.Enqueue("først", 1);
        queue.Enqueue("midt", 5);
        queue.Enqueue("først også", 1);

        Assert.Equal(["først", "først også", "midt", "sist"], queue.ToList());
    }

    // --- Iterasjon tømmer køen --------------------------------------------

    // GetEnumerator kaller Dequeue, så en foreach spiser opp køen underveis.
    // Det er et bevisst valg i ReviewQueue, og verdt å teste så ingen blir overrasket.
    [Fact]
    public void Enumeration_DrainsTheQueue()
    {
        ReviewQueue<string> queue = [];
        queue.Enqueue("a", NewPriority);
        queue.Enqueue("b", NewPriority);

        List<string> seen = [.. queue];

        Assert.Equal(["a", "b"], seen);
        Assert.True(queue.IsEmpty);
    }

    // Å legge inn nye elementer midt i en foreach er normalt forbudt, men her er det poenget:
    // "a igjen" blir med i samme gjennomgang, bakerst. Det er nettopp dette QuizSession
    // lener seg på når et feil svar skal stilles på nytt før quizen er ferdig.
    [Fact]
    public void Enqueue_DuringEnumeration_IsPickedUpLater()
    {
        ReviewQueue<string> queue = [];
        queue.Enqueue("a", NewPriority);
        queue.Enqueue("b", NewPriority);

        List<string> seen = [];

        foreach (string item in queue)
        {
            seen.Add(item);

            if (item == "a")
            {
                queue.Enqueue("a igjen", ReviewPriority);
            }
        }

        Assert.Equal(["a", "b", "a igjen"], seen);
        Assert.True(queue.IsEmpty);
    }
}
