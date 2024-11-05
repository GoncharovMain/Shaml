using Shaml.Tokens;

namespace Shaml.Extension
{
	public static class ReadOnlySpanExtensions
	{
		public static string Slice(this ReadOnlySpan<char> span, Mark mark)
			=> span.Slice(mark.Start, mark.Length).ToString();

		public static (string key, string value) Slice(this ReadOnlySpan<char> span, Pair pair)
		{
			string key = span.Slice(pair.Key);
			string value = span.Slice(pair.Value);

			return (key, value);
		}
	}
}