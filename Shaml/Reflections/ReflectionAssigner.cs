namespace Shaml.Reflections
{
	public class ReflectionAssigner
	{
		public SetterDelegate SetValue { get; init; }
		public Type MemberType { get; init; }
		public bool IsString => MemberType == typeof(string);
	}
}