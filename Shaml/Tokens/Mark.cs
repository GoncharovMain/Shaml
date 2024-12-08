namespace Shaml.Tokens
{
	public readonly struct Mark
	{
		public Mark() : this(0, 0) {}
		public Mark(int start, int end) => (Start, End) = (start, end);
		public readonly int Start;
		public readonly int End;
		public Range Range => Start..End;
		public int Length => End - Start + 1;
		public override string ToString() => $"[{Start}:{End}:{Length}]";
	}
}