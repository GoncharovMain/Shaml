namespace Shaml.Reflections
{
	public class ReflectionAssigner
	{
		private readonly object _instance;

		private readonly GetterDelegate _getValue;
		private readonly SetterDelegate _setValue;
		public Type MemberType { get; init; }
		public bool IsString => MemberType == typeof(string);

		public ReflectionAssigner(object instance, GetterDelegate getValue, SetterDelegate setValue)
		{
			_instance = instance;
			_getValue = getValue;
			_setValue = setValue;
		}

		public void SetValue(object value)
		{
			_setValue(_instance, value);
		}

		public object GetValue()
		{
			return _getValue(_instance);
		}

		public bool IsContainsValue => GetValue() != null;
	}
}