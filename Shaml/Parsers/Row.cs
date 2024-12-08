using Shaml.Tokens;

namespace Shaml.Parsers;

public class Row
{
    private readonly ReadOnlyMemory<char> _buffer;
    public readonly int IndentLevel = 0;
    
    public Eol Eol { get; }
    public Mark Mark { get; }

    public Row(ReadOnlyMemory<char> buffer, Mark mark, int indentLevel, Eol eol)
    {
        Mark = mark;
        _buffer = buffer;

        IndentLevel = indentLevel;
        Eol = eol;
    }

    public override string ToString()
    {
        return _buffer.Span.Slice(Mark.Start, Mark.Length).ToString();
    }
}