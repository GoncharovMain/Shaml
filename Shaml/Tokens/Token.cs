using System.Reflection;
using Shaml.Reflections;

namespace Shaml.Tokens
{
	public abstract class Token
	{
		public const BindingFlags PublicMembers =
			BindingFlags.Instance |
			BindingFlags.Public |
			BindingFlags.SetProperty |
			BindingFlags.SetField;

		public const MemberTypes SetterMemberTypes =
			MemberTypes.Property |
			MemberTypes.Field;

		public static readonly Type[] PrimitiveTypes =
		{
			typeof(byte),
			typeof(ushort),
			typeof(short),
			typeof(uint),
			typeof(int),
			typeof(ulong),
			typeof(long),
			typeof(float),
			typeof(double),
			typeof(decimal),
			typeof(bool),
			typeof(DateTime),
		};

		protected ReadOnlyMemory<char> _buffer;

		public abstract TokenType Type { get; }
		public Token(ReadOnlyMemory<char> buffer) => _buffer = buffer;

		internal abstract void Assign(ReflectionAssignerBuilder builder);
		public abstract string ToObject();
		// public T ToObject<T>();
	}
}