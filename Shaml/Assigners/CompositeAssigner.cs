using Shaml.Tokens;

namespace Shaml.Assigners;

public class CompositeAssigner : IAssigner
{
    private readonly Type _type;
    private readonly CompositeScalar _scalar;
    public Cache Cache { get; private set; }
    public CompositeAssigner(Type type, CompositeScalar scalar)
    {
        _type = type;
        _scalar = scalar;
        Cache = new Cache()
        {
            Type = _type,
        };
    }

    public void Assign(ref object instance)
    {
        /// Save in cache.
        Cache.Instance = instance;
    }

    public void InitializeContext(string pathRoot, Dictionary<string, Cache> globalContext)
    {
        globalContext[pathRoot] = Cache;
    }

    public T ToObject<T>() => (T)Cache.Instance;
}