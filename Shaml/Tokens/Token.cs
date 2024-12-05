using System.Reflection;
using Shaml.Reflections;

namespace Shaml.Tokens
{
	public abstract class Token
	{
		protected ReadOnlyMemory<char> _buffer;
		
		public const BindingFlags PublicMembers =
			BindingFlags.Instance |
			BindingFlags.Public |
			BindingFlags.SetProperty |
			BindingFlags.SetField;

		public const MemberTypes SetterMemberTypes =
			MemberTypes.Property |
			MemberTypes.Field;
		
		public abstract TokenType Type { get; }
		
		protected Token(ReadOnlyMemory<char> buffer) => _buffer = buffer;
		
		internal abstract object CreateInstance(Type type);

	}
}