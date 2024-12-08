using System.Text;

namespace Shaml.Tokens;

public class CompositeScalar : Token
{
    private readonly Mark[] _marks;
    
    public CompositeScalar(ReadOnlyMemory<char> buffer, Mark[] marks) : base(buffer)
    {
        _marks = marks;
    }

    public override TokenType Type => TokenType.Composite;
    internal override object CreateInstance(Type type)
    {
        StringBuilder stringBuilder = new();

        ReadOnlySpan<char> span = _buffer.Span;
        
        foreach (Mark mark in _marks)
        {
            stringBuilder.AppendLine(span.Slice(mark.Start, mark.Length).ToString());
        }

        return stringBuilder.ToString();
    }
}