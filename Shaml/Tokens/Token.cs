using System.Reflection;

namespace Shaml.Tokens
{
	public interface IToken
	{
		TokenType Type { get; }
	}

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
		public Token(ReadOnlyMemory<char> buffer) => _buffer = buffer;
	}
}