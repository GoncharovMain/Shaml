using System.Reflection;
using Shaml.Extension;
using Shaml.Reflections;

namespace Shaml.Tokens
{
	public class Pair : Token
	{
		public override TokenType Type => TokenType.Pair;
		public Mark Key { get; set; }
		public Mark Value { get; set; }
		public Pair(ReadOnlyMemory<char> buffer) : base(buffer) { }

		internal override void Assign(ReflectionAssignerBuilder reflectionAssignerBuilder)
		{
			ReadOnlySpan<char> span = _buffer.Span;

			string memberName = span.Slice(Key);

			ReflectionAssigner reflectionAssigner = reflectionAssignerBuilder.Build(memberName);

			if (reflectionAssigner == null || reflectionAssigner.IsContainsValue)
			{
				return;
			}

			string value = span.Slice(Value);

			switch (reflectionAssigner)
			{
				case ReflectionAssigner when reflectionAssigner.MemberType == typeof(DateTime):
				case { MemberType.IsPrimitive: true }:

					MethodInfo method_tryParse = reflectionAssigner.MemberType.GetMethod(
						name: "TryParse",
						types: new[] { typeof(string), reflectionAssigner.MemberType.MakeByRefType() }
					);

					if (method_tryParse != null)
					{
						object[] parameters = new object[] { value, null };

						bool success = (bool)method_tryParse.Invoke(null, parameters);

						if (success)
						{
							reflectionAssigner.SetValue(parameters[1]);
						}
					}

					break;

				case { IsString: true }:

					reflectionAssigner.SetValue(value);

					break;
				default:
					throw new NotImplementedException($"Not supported {reflectionAssigner.MemberType} for {Type} token");
			}
		}

		public override string ToObject()
		{
			return $"{_buffer.Span.Slice(Value)}";
		}
	}
}