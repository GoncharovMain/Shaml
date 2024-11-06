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
			public ShamlBenchmark()
			{
				using StreamReader reader = new("request.shaml");

				_text = reader.ReadToEnd();

				_buffer = _text.AsMemory();

				Node node = ShamlConverter.Parse(_buffer);
			}

			[Benchmark]
			public void Deserialize()
			{
				GoogleApi googleApi = ShamlConverter.Deserialize<GoogleApi>(_buffer);
			}

			[Benchmark]
			public void Assign()
			{
				GoogleApi googleApi = new()
				{
					Text = "Hello world",
					Request = new()
					{
						Body = "hello body",
						Query = new()
						{
							{ "key", "value" },
							{ "key2", "value2" },
						}
					}
				};

				ShamlAssigner<GoogleApi> assignerGoogleApi = new(_buffer);

				string[] values =
				{
					assignerGoogleApi.SelectToken("Request.Body").ToObject(),
					assignerGoogleApi.SelectToken("Request.Headers.SecFetchSite").ToObject(),
					assignerGoogleApi.SelectToken("User.Name").ToObject(),
					assignerGoogleApi.SelectToken("Request.Headers").ToObject(),
					assignerGoogleApi.SelectToken("List.0").ToObject(),
				};

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