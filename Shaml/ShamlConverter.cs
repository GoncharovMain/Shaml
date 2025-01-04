using Shaml.Assigners;
using Shaml.Parsers;
using Shaml.Tokens;

namespace Shaml
{
	public static class ShamlConverter
	{
		public static Node Parse(ReadOnlyMemory<char> buffer, Dictionary<string, Cache> globalContext)
		{
			Node node = Parser.Parse(buffer, globalContext);

			return node;
		}

		public static T Deserialize<T>(ReadOnlyMemory<char> buffer) where T : new()
		{
			ShamlAssigner<T> shamlAssigner = new(buffer);

			T t = new();
			
			shamlAssigner.Assign(t);

			return t;
		}
	}
}