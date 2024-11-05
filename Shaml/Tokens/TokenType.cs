namespace Shaml.Tokens
{
	[Flags]
	public enum TokenType : long
	{
		Pair = 1 << 1,
		SegmentPair = 1 << 2,
		Item = 1 << 3,
		Node = 1 << 4,
	}
}