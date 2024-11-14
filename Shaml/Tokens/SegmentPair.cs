using System.Reflection;
using System.Text;
using Shaml.Extension;
using Shaml.Models;
using Shaml.Reflections;

namespace Shaml.Tokens
{
	public class SegmentPair : Token
	{
		public override TokenType Type => TokenType.SegmentPair;
		public Mark Key { get; init; }
		public Mark[] Segments { get; init; }
		public SegmentPair(ReadOnlyMemory<char> buffer) : base(buffer) { }

		internal override void Assign(ReflectionAssignerBuilder reflectionAssignerBuilder)
		{
			string memberName = _buffer.Span.Slice(Key);

			ReflectionAssigner reflectionAssigner = reflectionAssignerBuilder.Build(memberName);

			if (reflectionAssigner == null || reflectionAssigner.IsContainsInstance)
			{
				return;
			}

			HtmlBodyConverterAttribute attributeConverter = reflectionAssigner.MemberType.GetCustomAttribute<HtmlBodyConverterAttribute>();

			if (attributeConverter != null)
			{
				HtmlBody htmlBody = new();

				//attributeConverter.Convert(htmlBody, this, reflectionAssigner);

				return;
			}

			string value = CollectSegments();

			reflectionAssigner.SetValue(value);
		}

		private string CollectSegments()
		{
			ReadOnlySpan<char> span = _buffer.Span;
			
			int length = Segments.Sum(segment => segment.Length);

			StringBuilder valueBuilder = new(length);


			for (int i = 0; i < Segments.Length; i++)
			{
				Mark segment = Segments[i];

				valueBuilder.Append(span.Slice(segment));
			}

			string value = valueBuilder.ToString();

			return value;
		}
		public override string ToObject()
		{
			string value = CollectSegments();

			return value;
		}
	}
}