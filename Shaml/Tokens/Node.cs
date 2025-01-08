using System.Reflection;
using System.Reflection.Metadata;
using Shaml.Assigners;
using Shaml.Extension;
using Shaml.Reflections;

namespace Shaml.Tokens
{
	public class Node : Token
	{
		public override TokenType Type => TokenType.Node;
		public Dictionary<IReference, Token> Children { get; }

		public Mark[] References { get; init; } = new Mark[0];

		public Node(ReadOnlyMemory<char> buffer) : base(buffer)
		{
			Children = new Dictionary<IReference, Token>(Assigner.Comparer);
		}
		
		internal override object CreateInstance(Type type)
		{
			return Activator.CreateInstance(type);
		}

		public Dictionary<string, Token> GetKeys()
		{
			Dictionary<string, Token> keys = new();
			
			foreach ((IReference reference, Token token) in Children)
			{
				keys.Add(reference.Literal, token);

				if (token is Node innerNode)
				{
					innerNode.GetKeys(reference.Literal, keys);
				}
			}

			return keys;
		}
		private void GetKeys(string rootKey, Dictionary<string, Token> keys)
		{
			foreach ((IReference reference, Token token) in Children)
			{
				string key = rootKey + Assigner.Dot + reference.Literal;
				
				keys.Add(key, token);
				
				if (token is Node node)
				{
					node.GetKeys(key, keys);
				}
			}
		}
		public void GetReferences(HashSet<string> references, Dictionary<IReference, string[]> referencesTree)
		{
			foreach ((IReference reference, Token token) in Children)
			{
				switch (token)
				{
					case Node node:
						node.GetReferences(references, referencesTree);
						break;
					case Scalar scalar:

						string[] innerReferences = scalar.Marks.Where(mark => mark.Type is MarkType.Reference)
							.Select(mark => _buffer.Span.Slice(mark)).ToArray();

						if (innerReferences.Length > 0)
						{
							referencesTree[reference] = innerReferences;
						}

						break;
				}
			}
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

		public void ImmediateReferences(List<string> references)
		{
			foreach ((IReference reference, Token token) in Children)
			{
				string key = reference.Literal;
				
				switch (token)
				{
					case CompositeScalar compositeScalar:
						if (compositeScalar.Marks.Any(mark => mark.Type is MarkType.Reference))
						{
							continue;
						}

						references.Add(key);
						break;
					case Scalar scalar:
						if (scalar.Marks.Any(mark => mark.Type is MarkType.Reference))
						{
							continue;
						}

						references.Add(key);

						break;
					case Node innerNode:
						if (innerNode.References.Length is 0)
						{
							references.Add(key);
						}
						
						innerNode.ImmediateReferences(key, references);
						break;
					
				}
			}
		}

		private void ImmediateReferences(string rootReference, List<string> references)
		{
			foreach ((IReference reference, Token token) in Children)
			{
				string key = rootReference + Assigner.Dot + reference.Literal;
				
				switch (token)
				{
					case Scalar scalar:
						if (scalar.Marks.Any(mark => mark.Type is MarkType.Reference))
						{
							continue;
						}

						references.Add(key);

						break;

					case Node innerNode:
						if (innerNode.References.Length is 0)
						{
							references.Add(key);
						}
						
						innerNode.ImmediateReferences(key, references);
						break;
				}
			}
		}
		
		public Node FilterByImmediate()
		{
			Node immediate = new(_buffer);
			
			if (Children.Count is 0)
			{
				return immediate;
			}

			foreach ((IReference reference, Token token) in Children)
			{
				switch (token)
				{
					case Scalar scalar:
						if (scalar.Marks.All(mark => mark.Type is not MarkType.Reference))
						{
							immediate.Children.Add(reference, scalar);
						}
						break;
					
					case CompositeScalar compositeScalar:
						immediate.Children.Add(reference, compositeScalar);
						break;
					
					case Node node:

						if (Children.Count is 0)
						{
							break;
						}
						
						Node immediateInner = node.FilterByImmediate();
						
						immediate.Children.Add(reference, immediateInner);
						break;
				}
			}

			return immediate;
		}

		public Node FilterByDeferred()
		{
			Node deferred = new(_buffer);

			if (Children.Count is 0)
			{
				return deferred;
			}

			foreach ((IReference reference, Token token) in Children)
			{
				switch (token)
				{
					case Scalar scalar:
						if (scalar.Marks.Any(mark => mark.Type is MarkType.Reference))
						{
							deferred.Children.Add(reference, scalar);
						}
						break;
					
					case CompositeScalar compositeScalar:
						deferred.Children.Add(reference, compositeScalar);
						break;
					
					case Node node:

						if (Children.Count is 0)
						{
							break;
						}
						
						Node immediateInner = node.FilterByDeferred();
						
						deferred.Children.Add(reference, immediateInner);
						break;
				}
			}
			
			return deferred;
		}
	}
}