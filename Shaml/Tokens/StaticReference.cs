using Shaml.Extension;

namespace Shaml.Tokens;

public class StaticReference : Scalar, IReference
{ 
    public override TokenType Type => TokenType.Static | TokenType.Key;
    public string Literal => _buffer.Span.Slice(Entire);
    public StaticReference(Mark key, ReadOnlyMemory<char> buffer)
        : base(buffer, new())
    {
        _buffer = buffer;
        Entire = key;
    }

    public override string ToString() => Literal;
}