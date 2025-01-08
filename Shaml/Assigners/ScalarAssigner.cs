using System.Diagnostics.CodeAnalysis;
using Shaml.Tokens;

namespace Shaml.Assigners;

internal sealed class ScalarAssigner : IAssigner
{
    private readonly Scalar _scalar;
    public Cache Cache { get; private set; }
    public ScalarAssigner(Type type, Scalar scalar)
    {
        _scalar = scalar;
        
        Cache = new Cache()
        {
            Type = type,
        };
    }
    
    public void Assign([NotNull] ref object instance) { }

    public void InitializeContext(string pathRoot, Dictionary<string, Cache> globalContext)
    {
        globalContext.Add(pathRoot, Cache);
    }

    public T ToObject<T>() => (T)Cache.Instance;
}