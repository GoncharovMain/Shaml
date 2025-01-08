namespace Shaml.Assigners;

public class Cache
{
    public object Instance { get; set; }
    public Type Type { get; set; }

    public override string ToString()
    {
        return Type.Name + ": " + Instance?.ToString();
    }
}