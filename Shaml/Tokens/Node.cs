using System.Reflection;
using Shaml.Assigners;
using Shaml.Extension;
using Shaml.Reflections;

namespace Shaml.Tokens
{
	public class Node : Token
	{
		public override TokenType Type => TokenType.Node;
		public Dictionary<IReference, Token> Children { get; }

		public Node(ReadOnlyMemory<char> buffer) : base(buffer)
		{
			Children = new Dictionary<IReference, Token>(Assigner.Comparer);
		}
		
		internal override object CreateInstance(Type type)
		{
			return Activator.CreateInstance(type);
		}
		
		public Token this[string path]
		{
			get
			{
				ReadOnlyMemory<char> buffer = path.AsMemory();

				StaticReference reference = new(new Mark(0, path.Length - 1), buffer);

				Children.TryGetValue(reference, out Token token);

				return token;
			}
		}
	}
}