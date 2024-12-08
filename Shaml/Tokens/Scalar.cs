using System.Globalization;
using System.Reflection;
using Shaml.Extension;
using Shaml.Reflections;

namespace Shaml.Tokens;

public class Scalar : Token
{
	private const string Hex = "0x";
	private const string Binary = "0b";
    public override TokenType Type => TokenType.Scalar;

    public Mark Value { get; init; }
    public Scalar(ReadOnlyMemory<char> buffer) : base(buffer) { }
    internal override object CreateInstance(Type type)
    {
	    ShamlType shamlType = new(type);
        ReadOnlySpan<char> span = _buffer.Span;
        
		switch (shamlType)
		{
			case { IsInteger: true }:
			{
				ReadOnlySpan<char> notation = span.Slice(Value.Start, 2);
				
				MethodInfo method_tryParse = shamlType.MemberType.GetMethod(
					name: "TryParse",
					types: new[] { typeof(string), typeof(NumberStyles), typeof(CultureInfo), shamlType.MemberType.MakeByRefType() }
				);
				
				switch (notation)
				{
					case Hex:
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
							return parameters[3];
						}
						
						break;
					}
					case Binary:
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
							return parameters[3];
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
							return parameters[3];
						}
						
						break;
					}
				}
				
				break;
			}
			case { IsNumber: true }:
			{
				string value = span.Slice(Value.Start, Value.Length).ToString();
				
				MethodInfo method_tryParse = shamlType.MemberType.GetMethod(
					name: "TryParse",
					types: new[] { typeof(string), typeof(NumberStyles), typeof(CultureInfo), shamlType.MemberType.MakeByRefType() }
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
						return parameters[3];
					}
				}
			
				break;
			}
			case { TypeCode: ShamlTypeCode.Bool }:
			{
				string value = span.Slice(Value).ToLower();

				bool flag = value == "true";

				return flag;
			}
			case { TypeCode: ShamlTypeCode.DateTime }:
			{
				string dateTime = span.Slice(Value);
				
				MethodInfo method_tryParse = shamlType.MemberType.GetMethod(
					name: "TryParse",
					types: new[] { typeof(string), shamlType.MemberType.MakeByRefType() }
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
						return parameters[1];
					}
				}
				
				break;
			}
			case { TypeCode: ShamlTypeCode.String }:
			{
				string value = span.Slice(Value);

				return value;
			}
			case { TypeCode: ShamlTypeCode.Char }:
			{
				char ch = span.Slice(Value)[0];

				return ch;
			}
			default:
				throw new NotImplementedException($"Not supported {type.MemberType} for {Type} token");
		}

		return default;
    }

    public override string ToString() => _buffer.Span.Slice(Value.Start, Value.Length).ToString();
}