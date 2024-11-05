using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Shaml.Models;
using Shaml.Tokens;

namespace Shaml.Samples
{
	public class Program
	{
		public class ShamlBenchmark
		{
			private readonly string _text;
			private readonly ReadOnlyMemory<char> _buffer;
			private readonly IToken[] _collection;
			public ShamlBenchmark()
			{
				using StreamReader reader = new("request.shaml");

				_text = reader.ReadToEnd();

				_buffer = _text.AsMemory();

				_collection = new IToken[]
				{
				new Node(_buffer)
				{
					/// Request
					Key = new Mark(0, 6),
					Collection = new IToken[]
					{
						/// Url: google.com
						new Pair() { Key = new Mark(10, 12), Value = new Mark(15, 24) },
						new Node(_buffer)
						{
							/// Query
							Key = new Mark(27, 31),
							Collection = new IToken[]
							{
								/// name: John
								new Pair() { Key = new Mark(36, 39), Value = new Mark(42, 45) },
								/// age: 18
								new Pair() { Key = new Mark(49, 51), Value = new Mark(54, 55) },
							}
						},
						new Node(_buffer)
						{
							/// Headers
							Key = new Mark(58, 64),
							Collection = new IToken[]
							{
								/// SecFetchSite: site
								new Pair() { Key = new Mark(69, 80), Value = new Mark(83, 86) },
								/// SecFetchDesc: user
								new Pair() { Key = new Mark(90, 101), Value = new Mark(104, 107) },
								/// SecFetchMode: one
								new Pair() { Key = new Mark(111, 122), Value = new Mark(125, 127) },
							}
						},
						new SegmentPair()
						{
							/// Body
							Key = new Mark(130, 133),
							Segments = new Mark[]
							{
								new Mark(144, 163),
								new Mark(167, 195),
							}
						},
					},
				},
				new Node(_buffer)
				{
					/// User
					Key = new Mark(204, 207),
					Collection = new IToken[]
					{
						/// Name: John
						new Pair() { Key = new Mark(211, 214), Value = new Mark(217, 220) },
						/// Age: 18
						new Pair() { Key = new Mark(223, 225), Value = new Mark(228, 229) },
					}
				},
				new Node(_buffer)
				{
					/// List
					Key = new Mark(232, 235),
					Collection = new IToken[]
					{
						/// item1
						new Item() { Value = new Mark(241, 245) },
						/// item2
						new Item() { Value = new Mark(250, 254) },
						/// item3
						new Item() { Value = new Mark(259, 263) }
					}
				}
				};
			}

			[Benchmark]
			public void Deserialize()
			{
				GoogleApi googleApi = ShamlConverter.Deserialize<GoogleApi>(_buffer, _collection);
			}
		}


		public static void Main(string[] args)
		{
			BenchmarkRunner.Run<ShamlBenchmark>();

			//new ShamlBenchmark().Deserialize();

		}
	}
}