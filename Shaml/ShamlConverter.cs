using Shaml.Tokens;

namespace Shaml
{
	public class ShamlAssigner<TInstance>
	{
		private readonly Node _node;
		private readonly ReadOnlyMemory<char> _buffer;
		private readonly Type _type;

		public ShamlAssigner(ReadOnlyMemory<char> buffer)
		{
			_buffer = buffer;
			_node = ShamlConverter.Parse(_buffer);
			_type = typeof(TInstance);
		}

		public Token SelectToken(string tokenPath)
		{
			string[] keys = tokenPath.Split('.');

			Token token = _node;

			foreach (string key in keys)
			{
				if (token is Node node)
				{
					token = node[key];
					continue;
				}
				return null;
			}

			return token;
		}

		public TInstance Assign(TInstance instance)
		{
			_node.Assign(instance);

			return instance;
		}
	}

	public static class ShamlConverter
	{
		public static Node Parse(ReadOnlyMemory<char> buffer)
		{
			Token[] tokens = new Token[]
			{
				new Node(buffer)
				{
					/// Request
					Key = new Mark(0, 6),
					Collection = new Token[]
					{
						/// Url: google.com
						new Pair(buffer) { Key = new Mark(10, 12), Value = new Mark(15, 24) },
						new Node(buffer)
						{
							/// Query
							Key = new Mark(27, 31),
							Collection = new Token[]
							{
								/// name: John
								new Pair(buffer) { Key = new Mark(36, 39), Value = new Mark(42, 45) },
								/// age: 18
								new Pair(buffer) { Key = new Mark(49, 51), Value = new Mark(54, 55) },
							}
						},
						new Node(buffer)
						{
							/// Headers
							Key = new Mark(58, 64),
							Collection = new Token[]
							{
								/// SecFetchSite: site
								new Pair(buffer) { Key = new Mark(69, 80), Value = new Mark(83, 86) },
								/// SecFetchDesc: user
								new Pair(buffer) { Key = new Mark(90, 101), Value = new Mark(104, 107) },
								/// SecFetchMode: one
								new Pair(buffer) { Key = new Mark(111, 122), Value = new Mark(125, 127) },
							}
						},
						new SegmentPair(buffer)
						{
							/// Body
							Key = new Mark(130, 133),
							Segments = new Mark[]
							{
								new Mark(144, 164),
								new Mark(167, 195),
							}
						},
					},
				},
				new Node(buffer)
				{
					/// User
					Key = new Mark(204, 207),
					Collection = new Token[]
					{
						/// Name: John
						new Pair(buffer) { Key = new Mark(211, 214), Value = new Mark(217, 220) },
						/// Age: 18
						new Pair(buffer) { Key = new Mark(223, 225), Value = new Mark(228, 229) },
					}
				},
				new Node(buffer)
				{
					/// List
					Key = new Mark(232, 235),
					Collection = new Token[]
					{
						/// item1
						new Item(buffer) { Index = 0, Value = new Mark(241, 245) },
						/// item2
						new Item(buffer) { Index = 1, Value = new Mark(250, 254) },
						/// item3
						new Item(buffer) { Index = 2, Value = new Mark(259, 263) }
					}
				}
			};

			Node node = new(buffer)
			{
				/// It is worth noting that the key on the topmost node is not used.
				/// Here a shell (<see cref="Node"/>) is created around the 
				/// <see cref="Token[]"/> collection.

				Key = new Mark(0, 0),
				Collection = tokens
			};

			return node;
		}

		public static T Deserialize<T>(ReadOnlyMemory<char> buffer)
		{
			Node node = Parse(buffer);

			return (T)node.CreateInstance(typeof(T));
		}
	}
}