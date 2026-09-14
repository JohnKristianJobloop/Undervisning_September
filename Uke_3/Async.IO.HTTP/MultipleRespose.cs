namespace Async.IO.HTTP;


// Ytterste nivå i JSON-svaret fra jokeapi. Navnene må matche feltene i JSON-en.
public class MultipleResponse
{
    public bool Error {get;set;}
    public int amount {get;set;}
    public Response[] Jokes{get;set;} = default!; // fylles av deserialiseringen, derfor default!
}