using System.Text;

namespace Shaml.Tokens;

public class CompositeScalar : Token
{
    public Mark[] Marks { get; init; }

    public CompositeScalar(ReadOnlyMemory<char> buffer, Mark[] marks) : base(buffer)
    {
        Marks = marks;
    }

    public override TokenType Type => TokenType.Composite;
    internal override object CreateInstance(Type type)
    {
        StringBuilder stringBuilder = new();

        ReadOnlySpan<char> span = _buffer.Span;
        
        foreach (Mark mark in Marks)
        {
            stringBuilder.AppendLine(span.Slice(mark.Start, mark.Length).ToString());
        }

        return stringBuilder.ToString();
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new();

        ReadOnlySpan<char> span = _buffer.Span;
        
        foreach (Mark mark in Marks)
        {
            stringBuilder.AppendLine(span.Slice(mark.Start, mark.Length).ToString());
        }

        return stringBuilder.ToString();
    }
    
}