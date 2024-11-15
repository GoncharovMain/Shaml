using Shaml.Tokens;

namespace Shaml
{
	public class ShamlAssigner<TInstance>
	{
		private readonly Node _node;
		private readonly Type _type;

		public ShamlAssigner(ReadOnlyMemory<char> buffer)
		{
			_node = ShamlConverter.Parse(buffer);
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


			node = new(buffer)
			{
				Key = new Mark(0, 0),
				Collection = new Token[]
				{
					new Pair(buffer) { Key = new Mark(0, 11), Value = new Mark(14, 16) },
					new Pair(buffer) { Key = new Mark(19, 27), Value = new Mark(30, 32) },
					new Pair(buffer) { Key = new Mark(35, 48), Value = new Mark(51, 54) },
					new Pair(buffer) { Key = new Mark(57, 67), Value = new Mark(70, 73) },
					new Pair(buffer) { Key = new Mark(76, 88), Value = new Mark(91, 94) },
					new Pair(buffer) { Key = new Mark(97, 106), Value = new Mark(109, 112) },
					new Pair(buffer) { Key = new Mark(115, 126), Value = new Mark(129, 138) },
					new Pair(buffer) { Key = new Mark(141, 149), Value = new Mark(152, 185) },
					new Pair(buffer) { Key = new Mark(188, 198), Value = new Mark(201, 210) },
					new Pair(buffer) { Key = new Mark(213, 220), Value = new Mark(223, 232) },
					new Pair(buffer) { Key = new Mark(235, 247), Value = new Mark(250, 267) },
					new Pair(buffer) { Key = new Mark(270, 279), Value = new Mark(282, 299) },
					new Pair(buffer) { Key = new Mark(302, 313), Value = new Mark(316, 334) },
					new Pair(buffer) { Key = new Mark(337, 345), Value = new Mark(348, 366) },
					new Pair(buffer) { Key = new Mark(369, 381), Value = new Mark(384, 389) },
					new Pair(buffer) { Key = new Mark(392, 401), Value = new Mark(404, 409) },
					new Pair(buffer) { Key = new Mark(412, 425), Value = new Mark(428, 444) },
					new Pair(buffer) { Key = new Mark(447, 457), Value = new Mark(460, 476) },
					new Pair(buffer) { Key = new Mark(479, 493), Value = new Mark(496, 512) },
					new Pair(buffer) { Key = new Mark(515, 526), Value = new Mark(529, 545) },
					new Pair(buffer) { Key = new Mark(548, 559), Value = new Mark(562, 565) },
					new Pair(buffer) { Key = new Mark(568, 576), Value = new Mark(579, 583) },
					new Pair(buffer) { Key = new Mark(586, 601), Value = new Mark(604, 621) },
					new Pair(buffer) { Key = new Mark(624, 636), Value = new Mark(639, 656) },
					new Pair(buffer) { Key = new Mark(659, 672), Value = new Mark(675, 695) },
					new Pair(buffer) { Key = new Mark(698, 708), Value = new Mark(711, 728) },
					new Pair(buffer) { Key = new Mark(731, 742), Value = new Mark(745, 745) },
					new Pair(buffer) { Key = new Mark(748, 756), Value = new Mark(759, 759) },
				}
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