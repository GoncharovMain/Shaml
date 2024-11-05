namespace Shaml.Tokens
{
	public class Pair : IToken
	{
		public TokenType Type => TokenType.Pair;
		public Mark Key { get; set; }
		public Mark Value { get; set; }
	}
}