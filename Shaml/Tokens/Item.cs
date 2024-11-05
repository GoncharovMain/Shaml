namespace Shaml.Tokens
{
	public class Item : IToken
	{
		public TokenType Type => TokenType.Item;
		public Mark Value { get; set; }
	}
}