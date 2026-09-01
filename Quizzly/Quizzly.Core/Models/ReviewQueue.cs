using System.Collections;

namespace Quizzly.Core.Models;


// Generisk kø: <T> gjør at samme klasse kan brukes til Question, int, eller hva som helst.
// IEnumerable<T> er det som gjør at vi kan skrive foreach rett på køen.
public class ReviewQueue<T>: IEnumerable<T>
{
    // Hvert element lagres sammen med prioriteten sin i en tuple. Lavt tall = tidlig i køen.
    private readonly List<(T Item, int Priority)> _pending = [];

    public int Count => _pending.Count;

    // Setter elementet inn bak alle som har lik eller lavere prioritet.
    public void Enqueue(T item, int priority)
    {
        int index = 0;

        while(index < _pending.Count && _pending[index].Priority <= priority)
        {
            index++;
        }

        _pending.Insert(index, (item, priority));
    }

    // Henter ut og fjerner det første elementet i køen.
    public T Dequeue()
    {
        if (_pending.Count == 0)
        {
            throw new InvalidOperationException("No elements in Queue");
        }

        T item = _pending[0].Item;
        _pending.RemoveAt(0);
        return item;
    }

    // yield return leverer ett element om gangen, i stedet for hele listen på en gang.
    // Fordi vi dequeuer underveis, får foreach også med spørsmål som legges inn på nytt.
    public IEnumerator<T> GetEnumerator()
    {
        while(_pending.Count > 0)
        {
            yield return Dequeue();
        }
    }

    // Den gamle, ikke-generiske versjonen kreves av IEnumerable. Den peker bare videre.
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}