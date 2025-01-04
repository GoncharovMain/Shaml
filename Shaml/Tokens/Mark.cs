namespace Shaml.Tokens
{
	[Flags]
	public enum MarkType : ulong
	{
		None = 1L << 1,
		Key = 2L << 2,
		Value = 3L << 3,
		Entire = 4L << 4,
		Scalar = 5L << 5,
		Reference = 6L << 6,
		Shield = 7L << 7,
		
		Row = 8L << 8,
		
		Eof = 9L << 9,
		LF = 10L << 10,
		CR = 11L << 11,
		CRLF = 12L << 12,
	}
	public readonly struct Mark
	{
		public readonly MarkType Type;
		public Mark() : this(0, 0, MarkType.None) {}
		public Mark(int position) : this(position, position, MarkType.Shield) { }
		public Mark(int position, MarkType type) : this(position, position, type) { }
		public Mark(int start, int end, MarkType type = MarkType.Scalar)
			=> (Start, End, Type) = (start, end, type);
		
		public readonly int Start;
		public readonly int End;
		public Range Range => Start..End;
		public int Length => End - Start + 1; 
		public override string ToString() => $"[{Start}:{End}:{Length}]";
	}
}