using System.Reflection;
using Shaml.Tokens;

namespace Shaml.Reflections
{
	public class ReflectionAssignerBuilder
	{
		private readonly Type _type;
		private MemberTypes _memberTypes;
		private BindingFlags _bindingFlags;

		public ReflectionAssignerBuilder(Type type)
		{
			_type = type;
			_memberTypes = Token.SetterMemberTypes;
			_bindingFlags = Token.PublicMembers;
		}

		public ReflectionAssignerBuilder SetMemberTypes(MemberTypes memberTypes)
		{
			_memberTypes = memberTypes;
			return this;
		}
		public ReflectionAssignerBuilder SetBindingAttribute(BindingFlags bindingFlags)
		{
			_bindingFlags = bindingFlags;
			return this;
		}

		public ReflectionAssigner Build(string memberName)
		{
			MemberInfo memberInfo = _type.GetMember(name: memberName,
				type: _memberTypes,
				bindingAttr: _bindingFlags).First();

			switch (memberInfo)
			{
				case PropertyInfo property:
					return new ReflectionAssigner()
					{
						SetValue = property.SetValue,
						MemberType = property.PropertyType,
					};

				case FieldInfo field:
					return new ReflectionAssigner()
					{

						SetValue = field.SetValue,
						MemberType = field.DeclaringType,
					};
				default:
					return null;
			};

		}
	}
}