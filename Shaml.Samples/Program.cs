using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Shaml.Assigners;
using Shaml.Models;
using Shaml.Tokens;

namespace Shaml.Samples
{
	public class Program
	{
		[MemoryDiagnoser, RankColumn]
		public class ShamlBenchmark
		{
			private readonly string _text;
			private readonly ReadOnlyMemory<char> _buffer;
			private readonly ShamlAssigner<GoogleApi> _assignerGoogleApi;
			
			public ShamlBenchmark()
			{
				using StreamReader reader = new("request.shaml");

				_text = reader.ReadToEnd();

				_buffer = _text.AsMemory();
				
				_assignerGoogleApi = new(_buffer);
			}

			[Benchmark]
			public void Deserialize()
			{
				GoogleApi googleApi = ShamlConverter.Deserialize<GoogleApi>(_buffer);
			}

			[Benchmark]
			public void Assign_2()
			{
				GoogleApi googleApi = new();
				
				_assignerGoogleApi.Assign(googleApi);
			}

			[Benchmark]
			public void Assign()
			{
				GoogleApi googleApi = new()
				{
					Text = "Hello world",
					Request = new Request
					{
						Body = "hello body",
						Query = new Dictionary<string, string>
						{
							{ "key", "value" },
							{ "key2", "value2" },
						}
					}
				};
				
				
				ShamlAssigner<GoogleApi> assignerGoogleApi = new(_buffer);
				
				assignerGoogleApi.Assign(googleApi);

				GoogleApi googleApi2 = assignerGoogleApi.Assign(new GoogleApi());
				
				GoogleApi googleApi3 = ShamlConverter.Deserialize<GoogleApi>(_buffer);
			}
		}

		public static void Main(string[] args)
		{
			//BenchmarkRunner.Run<ShamlBenchmark>();

			//new ShamlBenchmark().Deserialize();

			new ShamlBenchmark().Assign();
		}
	}
}