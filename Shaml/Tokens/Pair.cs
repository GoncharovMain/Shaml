using System.Globalization;
using System.Reflection;
using Shaml.Extension;
using Shaml.Reflections;

namespace Shaml.Tokens
{
	public class Pair : Token
	{
		public override TokenType Type => TokenType.Pair;
		public Mark Key { get; init; }
		public Mark Value { get; init; }
		public Pair(ReadOnlyMemory<char> buffer) : base(buffer) { }

		internal override void Assign(ReflectionAssignerBuilder reflectionAssignerBuilder)
		{
			ReadOnlySpan<char> span = _buffer.Span;

			string memberName = span.Slice(Key);

			ReflectionAssigner reflectionAssigner = reflectionAssignerBuilder.Build(memberName);

			if (reflectionAssigner == null || reflectionAssigner.IsContainsInstance)
			{
				return;
			}
			
			switch (reflectionAssigner)
			{
				case { IsInteger: true }:
				{
					ReadOnlySpan<char> notation = span.Slice(Value.Start, 2);
					
					MethodInfo method_tryParse = reflectionAssigner.MemberType.GetMethod(
						name: "TryParse",
						types: new[] { typeof(string), typeof(NumberStyles), typeof(CultureInfo), reflectionAssigner.MemberType.MakeByRefType() }
					);
					
					switch (notation)
					{
						case "0x":
						{
							ReadOnlySpan<char> numberSpan = span.Slice(Value.Start + 2, Value.Length - 2);
							
							object[] parameters = new object[]
							{
								numberSpan.ToString(),
								NumberStyles.HexNumber,
								CultureInfo.InvariantCulture,
								null
							};

							bool success = (bool)method_tryParse.Invoke(null, parameters);

							if (success)
							{
								reflectionAssigner.SetValue(parameters[3]);
							}
							
							break;
						}
						case "0b":
						{
							ReadOnlySpan<char> numberSpan = span.Slice(Value.Start + 2, Value.Length - 2);
							
							object[] parameters = new object[]
							{
								numberSpan.ToString(),
								NumberStyles.BinaryNumber,
								CultureInfo.InvariantCulture,
								null
							};

							bool success = (bool)method_tryParse.Invoke(null, parameters);

							if (success)
							{
								reflectionAssigner.SetValue(parameters[3]);
							}
							
							break;
						}
						default:
						{
							ReadOnlySpan<char> numberSpan = span.Slice(Value.Start, Value.Length);
							
							object[] parameters = new object[]
							{
								numberSpan.ToString(),
								NumberStyles.Any,
								CultureInfo.InvariantCulture,
								null
							};

							bool success = (bool)method_tryParse.Invoke(null, parameters);

							if (success)
							{
								reflectionAssigner.SetValue(parameters[3]);
							}
							
							break;
						}
					}
					
					break;
				}
				case { PrimitiveType: PrimitiveType.Bool }:
				{
					string value = span.Slice(Value).ToLower();

					bool flag = value == "true";
					
					reflectionAssigner.SetValue(flag);
					
					break;
				}
				
				case { IsNumber: true }:
				{
					string value = span.Slice(Value.Start, Value.Length).ToString();
					
					MethodInfo method_tryParse = reflectionAssigner.MemberType.GetMethod(
						name: "TryParse",
						types: new[] { typeof(string), typeof(NumberStyles), typeof(CultureInfo), reflectionAssigner.MemberType.MakeByRefType() }
					);

					if (method_tryParse != null)
					{
						object[] parameters = new object[]
						{
							value,
							NumberStyles.Any,
							CultureInfo.InvariantCulture,
							null
						};

						bool success = (bool)method_tryParse.Invoke(null, parameters);

						if (success)
						{
							reflectionAssigner.SetValue(parameters[3]);
						}
					}
				
					break;
				}
				case { PrimitiveType: PrimitiveType.DateTime }:
				{
					string dateTime = span.Slice(Value);
					
					MethodInfo method_tryParse = reflectionAssigner.MemberType.GetMethod(
						name: "TryParse",
						types: new[] { typeof(string), reflectionAssigner.MemberType.MakeByRefType() }
					);
					
					if (method_tryParse != null)
					{
						object[] parameters = new object[]
						{
							dateTime,
							null
						};

						bool success = (bool)method_tryParse.Invoke(null, parameters);

						if (success)
						{
							reflectionAssigner.SetValue(parameters[1]);
						}
					}
					
					break;
				}
				case { PrimitiveType: PrimitiveType.String }:
				{
					string value = span.Slice(Key);
					
					reflectionAssigner.SetValue(value);

					break;
				}
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