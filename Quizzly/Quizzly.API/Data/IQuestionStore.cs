using Quizzly.Core.Models;

namespace Quizzly.API.Data;

// Kontrakten for lagring av spørsmål: HVA API-et trenger, ikke HVORDAN det hentes.
// I dag er svaret en JSON-fil, i morgen kan det være en database - endepunktene merker ingenting.
public interface IQuestionStore
{
    // Async fordi lesing fra disk (eller nett) er I/O: tråden slipper å stå og vente.
    Task<IReadOnlyList<Question>> LoadAllAsync();
}
