namespace Quizzly.API.Models;

// Statusrapport for køen: hvor mange spørsmål som gjenstår, om den er tom,
// og hvilket spørsmål som står først. next er null når køen er tom -
// da finnes det ikke noe neste spørsmål å vise.
public sealed record ReviewQueueStatusResponse(int Count, bool IsEmpty, PendingQuestionResponse? next = null);
