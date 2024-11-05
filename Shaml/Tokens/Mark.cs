namespace Shaml
{
	public class Mark
	{
		public Mark(int start, int end) => (Start, End) = (start, end);
		public int Start { get; init; }
		public int End { get; init; }
		public int Length => End - Start + 1;

		public override string ToString() => $"[{Start}:{End}:{Length}]";
	}
}