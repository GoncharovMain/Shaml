namespace Shaml.Tokens
{
	public readonly struct Mark
	{
		public Mark() : this(0, 0) {}
		public Mark(int start, int end) => (Start, _end) = (start, end);
		public readonly int Start;
		private readonly int _end;
		public int Length => _end - Start + 1;
		public override string ToString() => $"[{Start}:{_end}:{Length}]";
	}
}