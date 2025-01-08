using BenchmarkDotNet.Attributes;
using Shaml.Assigners;
using Shaml.Models;
using Shaml.Parsers;
using Shaml.Tokens;
using Shaml.Extension;

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
			private readonly ReadOnlyMemory<char> _bufferShielding;
			private readonly ReadOnlyMemory<char> _bufferHtmlLayout;
			public ShamlBenchmark()
			{
				using StreamReader reader = new("request.shaml");
				using StreamReader htmlReader = new("html_layout.shaml");
				
				_text = reader.ReadToEnd();

				_buffer = _text.AsMemory();
				_bufferHtmlLayout = htmlReader.ReadToEnd().AsMemory();
				
				using StreamReader readerShielding = new("shielding.shaml");

				_bufferShielding = readerShielding.ReadToEnd().AsMemory();
				
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
				Node node = Parser.Parse(_buffer, new());

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
					},
					
					List = new()
					{
						"Hello"
					}
				};
				
				ShamlAssigner<HtmlLayout> assignerHtmlLayout = new(_bufferHtmlLayout);

				HtmlLayout htmlLayout = new();

				assignerHtmlLayout.Assign(htmlLayout);
				
				
				ShamlAssigner<GoogleApi> assignerGoogleApi = new(_buffer);
				
				assignerGoogleApi.Assign(googleApi);

				
				ShamlAssigner<GoogleApi> assignerGoogleApi2 = new(_buffer);

				GoogleApi googleApi2 = assignerGoogleApi2.Assign(new GoogleApi());

				
				GoogleApi googleApi3 = ShamlConverter.Deserialize<GoogleApi>(_buffer);
			}
		}

		public static void Main(string[] args)
		{
			//BenchmarkRunner.Run<ShamlBenchmark>();

			//new ShamlBenchmark().Deserialize();
			new ShamlBenchmark().Assign();
			
			//ShamlBenchmark.ParseReferenceMarks_IndexOf();
			//ShamlBenchmark.ParseReferenceMarks_For();
		}
	}
}