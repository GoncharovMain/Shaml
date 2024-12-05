namespace Shaml.Tokens;

public class ReferenceComparer : IEqualityComparer<IReference>
{
    public bool Equals(IReference x, IReference y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null)
        {
            return false;
        }

        if (y is null)
        {
            return false;
        }

        if (x.GetType() != y.GetType())
        {
            return false;
        }
        
        return x.Key == y.Key;
    }

    public int GetHashCode(IReference reference)
    {
        return (reference.Key != null ? reference.Key.GetHashCode() : 0);
    }
}