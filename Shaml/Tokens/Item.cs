using Shaml.Extension;
using Shaml.Reflections;

namespace Shaml.Tokens
{
	public class Item : Token, IToken
	{
		public int Index { get; init; }
		public override TokenType Type => TokenType.Item;
		public Mark Value { get; set; }
		public Item(ReadOnlyMemory<char> buffer) : base(buffer) { }
		internal override void Assign(ReflectionAssignerBuilder builder)
		{

		}

		public override string ToObject()
		{
			return $"{_buffer.Span.Slice(Value)}";
		}
	}
}