using Shaml.Tokens;

namespace Shaml.Extension
{
	public static class ReadOnlySpanExtensions
	{
		public static string Slice(this ReadOnlySpan<char> span, Mark mark)
			=> span.Slice(mark.Start, mark.Length).ToString();
	}
}