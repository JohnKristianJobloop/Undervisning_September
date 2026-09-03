using System.Diagnostics.CodeAnalysis;

namespace Quizzly.Core.Interfaces;

// Et interface er en kontrakt: det sier HVA en kø må kunne, ikke HVORDAN den gjør det.
// QuizSession ber om IReviewQueue<Question>, så vi kan bytte ut ReviewQueue med en
// helt annen implementasjon (f.eks. en test-kø med fast rekkefølge) uten å endre quizen.
// Arven fra IEnumerable<T> er det som gjør at vi kan skrive foreach rett på køen.
public interface IReviewQueue<T> : IEnumerable<T>
{
    // Interfaces forteller bare om signaturer, ingen implementasjon.
    int Count {get;}
    bool IsEmpty {get;}
    
    // Lavt prioritetstall = tidlig i køen.
    void Enqueue(T item, int priority);
    // Dequeue kaster hvis køen er tom, TryDequeue returnerer false i stedet.
    // Bruk Try-varianten når tom kø er en helt normal situasjon.
    T Dequeue();
    bool TryDequeue([MaybeNullWhen(false)] out T item);
    // Peek ser på det første elementet uten å fjerne det.
    T Peek();
}