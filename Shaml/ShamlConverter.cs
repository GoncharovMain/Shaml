using Shaml.Tokens;

namespace Shaml
{
	public class ShamlConverter
	{
		public static T Deserialize<T>(ReadOnlyMemory<char> buffer, IToken[] tokens)
		{
			Type type = typeof(T);

			Node node = new(buffer)
			{
				/// It is worth noting that the key on the topmost node is not used.
				/// Here a shell (<see cref="Node"/>) is created around the 
				/// <see cref="IToken[]"/> collection.

				Key = new Mark(0, 0),
				Collection = tokens
			};

			return (T)node.Assign(typeof(T));
		}
	}
}