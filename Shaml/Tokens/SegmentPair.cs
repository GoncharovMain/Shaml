namespace Shaml.Tokens
{
	public class SegmentPair : IToken
	{
		public TokenType Type => TokenType.SegmentPair;
		public Mark Key { get; set; }
		public Mark[] Segments { get; set; }
	}
}