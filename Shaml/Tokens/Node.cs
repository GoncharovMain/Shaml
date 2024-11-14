using System.Reflection;
using Shaml.Extension;
using Shaml.Reflections;

namespace Shaml.Tokens
{
	public class Node : Token
	{
		public Mark Key { get; init; }
		public Token[] Collection { get; init; }
		public override TokenType Type => TokenType.Node;
		public Node(ReadOnlyMemory<char> buffer) : base(buffer) { }
		public Token this[string path]
		{
			get
			{
				foreach (Token token in Collection)
				{
					switch (token)
					{
						case SegmentPair pair:
							{
								string key = _buffer.Span.Slice(pair.Key);

								if (key == path)
									return token;
								continue;
							}

						case Pair pair:
							{
								string key = _buffer.Span.Slice(pair.Key);

								if (key == path)
									return token;
								continue;
							}

						case Node node:
							{
								string key = _buffer.Span.Slice(node.Key);

								if (key == path)
									return token;
								continue;
							}

						case Item item:
							if (path == item.Index.ToString())
								return token;
							continue;
					}
				}

				return null;
			}
		}

		public override string ToObject()
		{
			return $"{_buffer.Span.Slice(Key)}: [node type({Collection.Length})]";
		}
		internal override void Assign(ReflectionAssignerBuilder reflectionAssignerBuilder)
		{
			ReadOnlySpan<char> span = _buffer.Span;

			string memberName = span.Slice(Key);

			ReflectionAssigner reflectionAssigner = reflectionAssignerBuilder.Build(memberName);

			if (reflectionAssigner == null)
			{
				return;
			}

			switch (reflectionAssigner)
			{
				case { MemberType.IsGenericType: true }:

					Type genericTypeDefinition = reflectionAssigner.MemberType.GetGenericTypeDefinition();

					switch (genericTypeDefinition)
					{
						case not null when genericTypeDefinition == typeof(Dictionary<,>):

							object dictionary = reflectionAssigner.GetValue();

							if (dictionary != null)
							{
								SupplementInstanceWithPairs(dictionary);
								break;
							}

							dictionary = CreateInstanceOfDictionary(reflectionAssigner.MemberType);

							reflectionAssigner.SetValue(dictionary);

							break;

						case not null when genericTypeDefinition == typeof(List<>):

							object list = CreateInstanceOfList(reflectionAssigner.MemberType);

							reflectionAssigner.SetValue(list);

							break;
					}
					break;

				case { MemberType.IsArray: true }:
					break;

				/// The built-in numeric types (int, long, byte), char,
				/// enums and structs are all value types.
				case { MemberType.IsValueType: false }:

					object memberInstance = reflectionAssigner.GetValue();

					if (memberInstance != null)
					{
						Assign(memberInstance);
						break;
					}

					memberInstance = CreateInstance(reflectionAssigner.MemberType);

					reflectionAssigner.SetValue(memberInstance);
					break;
			}
		}

		internal void Assign(object instance)
		{
			ReflectionAssignerBuilder reflectionAssignerBuilder =
				new ReflectionAssignerBuilder(instance)
					.SetMemberTypes(Token.SetterMemberTypes)
					.SetBindingAttribute(Token.PublicMembers);

			foreach (Token token in Collection)
			{
				token.Assign(reflectionAssignerBuilder);
			}
		}

		internal void SupplementInstanceWithPairs(object dictionary)
		{
			Type dictionaryType = dictionary.GetType();

			ReadOnlySpan<char> span = _buffer.Span;

			Type[] argumentsGenericTypes = dictionaryType.GetGenericArguments();

			Type keyType = argumentsGenericTypes[0];
			Type valueType = argumentsGenericTypes[1];

			if (Collection.Length == 0)
			{
				return;
			}

			MethodInfo method_add = dictionaryType.GetMethod("Add");

			switch (valueType)
			{
				case not null when keyType == typeof(string) && valueType == typeof(string):
				{
					foreach (Pair pair in Collection)
					{
						(string key, string value) = span.Slice(pair);

						method_add.Invoke(dictionary, new object[] { key, value });
					}

					break;
				}

				/// The primitive types are Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Char, Double, and Single.
				case not null when keyType.IsPrimitive && valueType.IsPrimitive:
				{
					MethodInfo methodKey_tryParse = GetTryParse(keyType);
					MethodInfo methodValue_tryParse = GetTryParse(valueType);

					MethodInfo GetTryParse(Type type)
					{
						return type.GetMethod(name: "TryParse", types: new[] { typeof(string), type });
					}

					if (methodKey_tryParse == null || methodValue_tryParse == null)
					{
						return;
					}

					/// What if is Token is SegmentsPair?
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
		}
		internal object CreateInstance(Type type)
		{
			object instance = Activator.CreateInstance(type);

			ReflectionAssignerBuilder reflectionAssignerBuilder =
				new ReflectionAssignerBuilder(instance)
					.SetMemberTypes(Token.SetterMemberTypes)
					.SetBindingAttribute(Token.PublicMembers);

			foreach (Token token in Collection)
			{
				token.Assign(reflectionAssignerBuilder);
			}

			return instance;
		}
		internal object CreateInstanceOfList(Type listType)
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
		internal object CreateInstanceOfDictionary(Type dictionaryType)
		{
			object dictionary = Activator.CreateInstance(dictionaryType, Collection.Length);

			SupplementInstanceWithPairs(dictionary);

			return dictionary;
		}
	}
}