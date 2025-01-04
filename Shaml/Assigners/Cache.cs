namespace Shaml.Assigners;

public class Cache
{
    public object Instance { get; set; }

    public override string ToString()
    {
        return Instance?.ToString();
    }
}