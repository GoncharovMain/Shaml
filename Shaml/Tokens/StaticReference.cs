using Shaml.Extension;

namespace Shaml.Tokens;

public class StaticReference : Scalar, IReference
{ 
    public override TokenType Type => TokenType.Static | TokenType.Key;
    public string Key => _buffer.Span.Slice(Value);
    public StaticReference(Mark key, ReadOnlyMemory<char> buffer)
        : base(buffer)
    {
        _buffer = buffer;
        Value = key;
    }

    public override string ToString() => Key;
}