using System.Text;
using System;
using Shaml.Reflections;
using Shaml.Extension;

namespace Shaml.Tokens
{
	public class SegmentPair : Token, IToken
	{
		public override TokenType Type => TokenType.SegmentPair;
		public Mark Key { get; set; }
		public Mark[] Segments { get; set; }
		public SegmentPair(ReadOnlyMemory<char> buffer) : base(buffer) { }

		internal override void Assign(ReflectionAssignerBuilder reflectionAssignerBuilder)
		{
			ReadOnlySpan<char> span = _buffer.Span;

			string memberName = span.Slice(Key);

			ReflectionAssigner reflectionAssigner = reflectionAssignerBuilder.Build(memberName);

			if (reflectionAssigner == null || reflectionAssigner.IsContainsValue)
			{
				return;
			}

			int length = Segments.Sum(segment => segment.Length);

			StringBuilder valueBuilder = new(length);


			for (int i = 0; i < Segments.Length; i++)
			{
				Mark segment = Segments[i];

				valueBuilder.Append(span.Slice(segment));
			}

			string value = valueBuilder.ToString();

			reflectionAssigner.SetValue(value);
		}

		public override string ToObject()
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
	}
}