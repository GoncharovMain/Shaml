using System.Diagnostics.CodeAnalysis;
using Shaml.Tokens;

namespace Shaml.Assigners;

internal sealed class ArrayAssigner : Assigner, IAssigner
{
    public ArrayAssigner(Type type, Dictionary<IReference, Token> tokens)
    {
        _type = type;

        Cache = new Cache()
        {
            Type = _type
        };
    }
    public override void Assign([NotNull] ref object array)
    {
        throw new NotImplementedException();
    }

    public override void InitializeContext(string pathRoot, Dictionary<string, Cache> globalContext)
    {
        globalContext[pathRoot] = Cache;
    }
}