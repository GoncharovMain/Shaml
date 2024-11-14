using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.InteropServices;
using Shaml.Tokens;
using System.Runtime.CompilerServices;

namespace Shaml.Reflections
{
	public class ReflectionAssignerBuilder
	{
		private readonly Type _type;
		private MemberTypes _memberTypes;
		private BindingFlags _bindingFlags;
		private readonly object _instance;
		public ReflectionAssignerBuilder(object instance)
		{
			_instance = instance;
			_type = instance.GetType();
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
				bindingAttr: _bindingFlags).FirstOrDefault();
			
			switch (memberInfo)
			{
				case PropertyInfo property:
					return new ReflectionAssigner(instance: _instance,
						memberType: property.PropertyType,
						getValue: property.GetValue,
						setValue: property.SetValue);

				case FieldInfo field:
					return new ReflectionAssigner(instance: _instance,
						memberType: field.FieldType,
						getValue: field.GetValue,
						setValue: field.SetValue);
				default:
					return null;
			};

		}
	}
}