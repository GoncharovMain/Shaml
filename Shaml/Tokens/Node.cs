using System.Reflection;
using System.Text;
using Shaml.Extension;
using Shaml.Reflections;

namespace Shaml.Tokens
{
	public class Node : Token, IToken
	{
		public Node(ReadOnlyMemory<char> buffer) : base(buffer) { }

		public TokenType Type => TokenType.Node;
		public Mark Key { get; init; }
		public IToken[] Collection { get; set; }

		public object Assign(Type type)
		{
			object instance = Activator.CreateInstance(type);

			ReflectionAssignerBuilder reflectionAssignerBuilder =
				new ReflectionAssignerBuilder(type)
					.SetMemberTypes(Token.SetterMemberTypes)
					.SetBindingAttribute(Token.PublicMembers);

			ReadOnlySpan<char> span = _buffer.Span;

			foreach (IToken token in Collection)
			{
				switch (token)
				{
					case Pair pair:
						{
							string memberName = span.Slice(pair.Key);

							ReflectionAssigner reflectionAssigner = reflectionAssignerBuilder.Build(memberName);

							if (reflectionAssigner == null)
							{
								break;
							}

							string value = span.Slice(pair.Value);

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
											reflectionAssigner.SetValue(instance, parameters[1]);
										}
									}

									break;

								case { IsString: true }:

									reflectionAssigner.SetValue(instance, value);

									break;
							}

							break;
						}

					case SegmentPair segmentPair:
						{
							string memberName = span.Slice(segmentPair.Key);

							ReflectionAssigner reflectionAssigner = reflectionAssignerBuilder.Build(memberName);


							int length = segmentPair.Segments.Sum(segment => segment.Length);

							StringBuilder valueBuilder = new(length);


							for (int i = 0; i < segmentPair.Segments.Length; i++)
							{
								Mark segment = segmentPair.Segments[i];

								valueBuilder.Append(span.Slice(segment));
							}

							reflectionAssigner.SetValue(instance, valueBuilder.ToString());

							break;
						}

					case Item item:
						{
							break;
						}

					case Node node:
						{
							string memberName = span.Slice(node.Key);

							ReflectionAssigner reflectionAssigner = reflectionAssignerBuilder.Build(memberName);

							if (reflectionAssigner == null)
							{
								break;
							}

							switch (reflectionAssigner)
							{
								case { MemberType.IsGenericType: true }:

									Type genericTypeDefinition = reflectionAssigner.MemberType.GetGenericTypeDefinition();

									switch (genericTypeDefinition)
									{
										case System.Type when genericTypeDefinition == typeof(Dictionary<,>):

											object dictionary = node.AssignDictionary(reflectionAssigner.MemberType);

											reflectionAssigner.SetValue(instance, dictionary);

											break;

										case System.Type when genericTypeDefinition == typeof(List<>):

											object list = node.AssignList(reflectionAssigner.MemberType);

											reflectionAssigner.SetValue(instance, list);

											break;
									}
									break;

								case { MemberType.IsArray: true }:
									break;

								/// The built-in numeric types (int, long, byte), char,
								/// enums and structs are all value types.
								case { MemberType.IsValueType: false }:

									object memberInstance = node.Assign(reflectionAssigner.MemberType);

									reflectionAssigner.SetValue(instance, memberInstance);
									break;
							}
							break;
						}
				}
			}
			return instance;
		}

		public object AssignList(Type listType)
		{
			ReadOnlySpan<char> span = _buffer.Span;

			Type itemType = listType.GetGenericArguments()[0];

			object list = Activator.CreateInstance(listType, Collection.Length);

			MethodInfo method_add = listType.GetMethod("Add");

			switch (itemType)
			{
				case System.Type when itemType == typeof(string):

					string[] values = new string[Collection.Length];

					foreach (Item item in Collection)
					{
						string value = span.Slice(item.Value);

						method_add.Invoke(list, new object[] { value });
					}

					break;

				case System.Type when itemType == typeof(DateTime):
				case System.Type when itemType.IsPrimitive:

					MethodInfo method_tryParse = itemType.GetMethod(
							name: "TryParse",
							types: new[] { typeof(string), itemType.MakeByRefType() }
						);

					if (method_tryParse == null)
					{
						return default;
					}

					foreach (Item item in Collection)
					{
						string value = span.Slice(item.Value);

						object[] parameters = new object[] { value, null };

						bool success = (bool)method_tryParse.Invoke(null, parameters);

						if (success)
						{
							method_add.Invoke(list, new object[] { parameters[1] });
						}
					}

					break;
			}


			return list;
		}
		public object AssignDictionary(Type dictionaryType)
		{
			ReadOnlySpan<char> span = _buffer.Span;

			Type[] argumentsGenericTypes = dictionaryType.GetGenericArguments();

			Type keyType = argumentsGenericTypes[0];
			Type valueType = argumentsGenericTypes[1];

			object dictionary = Activator.CreateInstance(dictionaryType, Collection.Length);

			if (Collection.Length == 0)
			{
				return dictionary;
			}

			MethodInfo method_add = dictionaryType.GetMethod("Add");

			switch (valueType)
			{
				case System.Type when keyType == typeof(string) && valueType == typeof(string):
					{
						foreach (Pair pair in Collection)
						{
							(string key, string value) = span.Slice(pair);

							method_add.Invoke(dictionary, new object[] { key, value });
						}

						break;
					}

				/// The primitive types are Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Char, Double, and Single.
				case System.Type when keyType.IsPrimitive && valueType.IsPrimitive:
					{
						MethodInfo methodKey_tryParse = keyType.GetMethod(
							name: "TryParse",
							types: new[] { typeof(string), keyType.MakeByRefType() }
						);

						MethodInfo methodValue_tryParse = valueType.GetMethod(
							name: "TryParse",
							types: new[] { typeof(string), valueType.MakeByRefType() }
						);


						if (methodKey_tryParse == null || methodValue_tryParse == null)
						{
							return default;
						}

						foreach (Pair pair in Collection)
						{
							(string key, string value) = span.Slice(pair);

							object[] parametersKey = new object[] { key, null };
							object[] parametersValue = new object[] { value, null };

							bool successKey = (bool)methodKey_tryParse.Invoke(null, parametersKey);
							bool successValue = (bool)methodValue_tryParse.Invoke(null, parametersValue);

							if (successKey && successValue)
							{
								method_add.Invoke(dictionary, new object[] { parametersKey[1], parametersValue[1] });
							}
						}

						break;
					}
			}

			return dictionary;
		}
	}
}