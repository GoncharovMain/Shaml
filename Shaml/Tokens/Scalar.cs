using System.ComponentModel.Design;
using System.Globalization;
using System.Reflection;
using System.Text;
using Shaml.Assigners;
using Shaml.Extension;
using Shaml.Reflections;

namespace Shaml.Tokens;

public class Scalar : Token
{
	private const string Hex = "0x";
	private const string Binary = "0b";
    public override TokenType Type => TokenType.Scalar;

    public Mark Entire { get; init; }
    public Mark[] Marks { get; init; } = new Mark[0];

    protected Dictionary<string, Cache> _globalContext;

    public Scalar(ReadOnlyMemory<char> buffer, Dictionary<string, Cache> context) : base(buffer)
    {
	    _globalContext = context ?? throw new ArgumentNullException(nameof(context));
    }
    internal override object CreateInstance(Type type)
    {
	    ShamlType shamlType = new(type);

	    string literal = BuildLiteral();
	    
		switch (shamlType)
		{
			case { IsInteger: true }:
			{
				ReadOnlySpan<char> notation = literal.Substring(0, 2);
				
				MethodInfo method_tryParse = shamlType.MemberType.GetMethod(
					name: "TryParse",
					types: new[] { typeof(string), typeof(NumberStyles), typeof(CultureInfo), shamlType.MemberType.MakeByRefType() }
				);
				
				switch (notation)
				{
					case Hex:
					{
						ReadOnlySpan<char> numberSpan = literal;
						
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
						ReadOnlySpan<char> numberSpan = literal;
						
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
						ReadOnlySpan<char> numberSpan = literal;
						
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
				MethodInfo method_tryParse = shamlType.MemberType.GetMethod(
					name: "TryParse",
					types: new[] { typeof(string), typeof(NumberStyles), typeof(CultureInfo), shamlType.MemberType.MakeByRefType() }
				);

				if (method_tryParse != null)
				{
					object[] parameters = new object[]
					{
						literal,
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
				literal = literal.ToLower();

				bool flag = literal == "true";

				return flag;
			}
			case { TypeCode: ShamlTypeCode.DateTime }:
			{
				string dateTime = literal;
				
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
				return literal;
			}
			case { TypeCode: ShamlTypeCode.Char }:
			{
				char ch = literal[0];

				return ch;
			}
			default:
				throw new NotImplementedException($"Not supported {type.MemberType} for {Type} token");
		}

		return default;
    }

    public string BuildLiteral()
    {
	    if (Marks.Length is 0)
	    {
		    return _buffer.Span.Slice(Entire);
	    }
	    
	    int length = 0;
	    ReadOnlySpan<char> span = _buffer.Span;

	    foreach (Mark mark in Marks)
	    {
		    switch (mark.Type)
		    {
			    case MarkType.Reference:
				    string k = span.Slice(mark.Start + 2, mark.Length - 3).ToString();
				    if (_globalContext.TryGetValue(k, out Cache v))
				    {
					    if (v.Instance is string str)
					    {
						    length += str.Length;
					    }
				    }
				    break;
			    
			    case MarkType.Scalar:
				    length += mark.Length;
				    break;
		    }
	    }

	    StringBuilder tokenBuilder = new(length);

	    foreach (Mark mark in Marks)
	    {
		    switch (mark.Type)
		    {
			    case MarkType.Scalar:
				    tokenBuilder.Append(span.Slice(mark));
				    break;
		        
			    case MarkType.Reference:
				    string key = span.Slice(mark.Start + 2, mark.Length - 3).ToString();
				    
				    if (_globalContext.TryGetValue(key, out Cache cache))
				    {
					    switch (cache.Instance)
					    {
						    case string str:
							    tokenBuilder.Append(str);
							    break;
						    
						    case null:
							    tokenBuilder.Append("[null]");
							    break;
						    
						    default:
							    tokenBuilder.Append($"[{cache.GetType().Name}]");
							    break;
					    }
				    }

				    break;
		    }
	    }

	    string value = tokenBuilder.ToString();
	    
	    return value;
    }

    public override string ToString() => _buffer.Span.Slice(Entire);
}