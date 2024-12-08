namespace Shaml.Tokens
{
	[Flags]
	public enum TokenType : uint
	{
		Node = 1U << 1,
		Scalar = 1U << 2,
		Composite = 1U << 3,
		Index = 1U << 4,
		Key = 1U << 5,
		Static = 1U << 6,
	}
}