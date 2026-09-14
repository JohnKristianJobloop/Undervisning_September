namespace Async.Common;


// Felles adresse for demoene, slik at klient og server ikke kommer i utakt.
public static class Protocol
{
    public const string Host = "0.0.0.0"; // lytt på alle nettverkskort på maskinen
    public const int Port = 5050;          // porten vi ber OS om
}