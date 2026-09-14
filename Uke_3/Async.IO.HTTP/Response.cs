namespace Async.IO.HTTP;

// Én vits fra API-et. Bare feltene vi faktisk bruker trenger å være her.
public class Response
{
    public int Id {get;set;}
    public string Language{get;set;} = string.Empty;
    public string Category{get;set;} = string.Empty;
    public string Type {get;set;} = string.Empty;
    public string Setup {get;set;} = string.Empty;
    public string Delivery {get;set;} = string.Empty;
    public ResponseFlags Flags{get;set;} = default!;

}


// Flaggene API-et bruker til å merke innhold, nøstet objekt inne i hver vits.
public class ResponseFlags
{
    public bool Nsfw {get;set;}
    public bool Religious {get;set;}
    public bool Political {get;set;}
    public bool Racist {get;set;}
    public bool Sexist {get;set;}
    public bool Explicit {get;set;}
}