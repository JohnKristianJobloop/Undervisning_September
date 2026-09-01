
var intContainer = new Container<int>();

intContainer.Value = 10;

var stringContainer = new Container<string>();
stringContainer.Value = "Hallo!";

var someClassContainer = new Container<SomeClass>();
someClassContainer.Value = new();






class SomeClass
{
    
}


class Container<T> where T : IComparable<T>
{
    public T Value {get;set;}
    public int CompareWithValue(T Other) => Value.CompareTo(Other);
}