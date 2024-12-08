using System.Diagnostics.CodeAnalysis;
using Shaml.Tokens;

namespace Shaml.Assigners;

public class ArrayAssigner : IAssigner
{
    private Type _type;

    public ArrayAssigner(Type type, Dictionary<IReference, Token> tokens)
    {
        _type = type;
    }
    public void Assign([NotNull] ref object array)
    {
        throw new NotImplementedException();
    }
}