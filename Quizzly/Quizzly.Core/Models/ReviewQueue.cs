using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Quizzly.Core.Interfaces;

namespace Quizzly.Core.Models;


// Generisk kø: <T> gjør at samme klasse kan brukes til Question, int, eller hva som helst.
// Klassen oppfyller kontrakten IReviewQueue<T>, som igjen arver IEnumerable<T>,
// derfor må vi implementere både kø-metodene og GetEnumerator.
public class ReviewQueue<T>: IReviewQueue<T>
{
    // Hvert element lagres sammen med prioriteten sin i en tuple. Lavt tall = tidlig i køen.
    private readonly List<(T Item, int Priority)> _pending = [];

    public int Count => _pending.Count;

    // En sannhet om "tom kø", brukt av både Dequeue, TryDequeue, Peek og foreach.
    public bool IsEmpty => Count == 0;

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

    // Henter ut og fjerner det første elementet i køen. Kaster hvis køen er tom.
    public T Dequeue()
    {
        ThrowIfEmpty();

        T item = _pending[0].Item;
        _pending.RemoveAt(0);
        return item;
    }

    // yield return leverer ett element om gangen, i stedet for hele listen på en gang.
    // NB: denne varianten tømmer køen mens du går gjennom den, og får derfor også med
    // elementer som legges inn på nytt underveis. Vil du bare kikke uten å tømme,
    // er det Peek du skal bruke.
    public IEnumerator<T> GetEnumerator()
    {
        while(!IsEmpty)
        {
            yield return Dequeue();
        }
    }

    // Den gamle, ikke-generiske versjonen kreves av IEnumerable. Den peker bare videre.
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // Try-mønsteret: tom kø er ikke en feil, bare et "nei" vi kan teste på i en while-løkke.
    // [MaybeNullWhen(false)] lover kompilatoren at item har verdi når vi returnerer true.
    public bool TryDequeue([MaybeNullWhen(false)] out T item)
    {
        // default er riktig "ingenting" uansett hva T er.
        if (IsEmpty)
        {
            item = default;
            return false;
        }
        item = Dequeue();
        return true;
    }

    // Ser på neste element uten å fjerne det fra køen.
    public T Peek()
    {
        ThrowIfEmpty();
        return _pending[0].Item;
    }

    // Privat hjelpemetode: samler feilmeldingen ett sted i stedet for å gjenta if-en.
    private void ThrowIfEmpty()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException("No elements in Queue");
        }
    }
}