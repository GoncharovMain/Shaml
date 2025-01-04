using System.Diagnostics.CodeAnalysis;
using Shaml.Tokens;

namespace Shaml.Assigners;

internal sealed class ArrayAssigner : IAssigner
{
    private Type _type;
    public Cache Cache { get; private set; }
    
    public ArrayAssigner(Type type, Dictionary<IReference, Token> tokens)
    {
        _type = type;
        
        Cache = new Cache();
    }
    public void Assign([NotNull] ref object array)
    {
        throw new NotImplementedException();
    }

    public void InitializeContext(string pathRoot, Dictionary<string, Cache> globalContext)
    {
        globalContext[pathRoot] = Cache;
    }
}