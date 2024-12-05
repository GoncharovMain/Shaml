namespace Shaml.Tokens;

public class IndexReference : Token, IReference
{
    public override TokenType Type => TokenType.Index | TokenType.Key;
    public int Index { get; }
    public string Key => Index.ToString();
    public IndexReference(int index, ReadOnlyMemory<char> buffer) : base(buffer) => Index = index;
    internal override object CreateInstance(Type type)
    {
        return Index;
    }
}