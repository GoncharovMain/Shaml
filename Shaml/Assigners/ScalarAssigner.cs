using System.Diagnostics.CodeAnalysis;
using Shaml.Reflections;
using Shaml.Tokens;

namespace Shaml.Assigners;

public class ScalarAssigner : IAssigner
{
    private object _instance;
    private readonly Type _type;
    private readonly Scalar _scalar;
    public ScalarAssigner(Type type, Scalar scalar)
    {
        _type = type;
        _scalar = scalar;
    }

    public void Assign([NotNull] ref object instance)
    {
        /// Save in cache.
        _instance = instance;
    }

    public T ToObject<T>() => (T)_instance;
}