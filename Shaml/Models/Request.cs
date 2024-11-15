using System.Text.Json.Nodes;
using HtmlAgilityPack;
using Shaml.Reflections;
using Shaml.Tokens;

namespace Shaml.Models
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public abstract class ShamlConverterAttribute<TInstance> : Attribute
	{
		public abstract void Convert(TInstance instance, Token token, ReflectionAssigner reflectionAssigner);
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
	public class HtmlBodyConverterAttribute : ShamlConverterAttribute<HtmlBody>
	{
		public override void Convert(HtmlBody instance, Token token, ReflectionAssigner reflectionAssigner)
		{
			throw new NotImplementedException();
		}
	}
	public interface IBody
	{
		string Text { get; set; }
	}
	public class JsonBody : IBody
	{
		public string Text { get; set; }
	}
	[HtmlBodyConverter]
	public class HtmlBody : IBody
	{
		public string Text { get; set; }

		private HtmlDocument _document;
		public HtmlBody()
		{
			HtmlDocument document = new();

		}
	}
	public class Request
	{
		public string Url { get; set; }
		public string Method { get; set; }
		public string MimeType { get; set; }
		public Dictionary<string, string> Query { get; set; }
		public Dictionary<string, string> Headers { get; set; }
		public HtmlBody Body { get; set; }
	}
}