using Shaml.Tokens;

namespace Shaml.Assigners;

public class CompositeAssigner : IAssigner
{
    private object _instance;
    private readonly Type _type;
    private readonly CompositeScalar _scalar;
    
    public CompositeAssigner(Type type, CompositeScalar scalar)
    {
        _type = type;
        _scalar = scalar;
    }
    public void Assign(ref object instance)
    {
        /// Save in cache.
        _instance = instance;
    }
    public T ToObject<T>() => (T)_instance;
}