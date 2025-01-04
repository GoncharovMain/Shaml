using System.Diagnostics.CodeAnalysis;
using Shaml.Tokens;

namespace Shaml.Assigners;

internal sealed class ScalarAssigner : IAssigner
{
    private readonly Type _type;
    private readonly Scalar _scalar;
    public Cache Cache { get; private set; }
    public ScalarAssigner(Type type, Scalar scalar)
    {
        _type = type;
        _scalar = scalar;
        Cache = new Cache();
    }
    
    public void Assign([NotNull] ref object instance)
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