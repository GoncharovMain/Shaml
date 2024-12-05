using System.Reflection;
using Shaml.Reflections;

namespace Shaml.Assigners;

public class MemberAssigner
{
    private readonly SetterDelegate _setValue;
    public Type Type { get; }

    public MemberAssigner(MemberInfo memberInfo)
    {
        switch (memberInfo)
        {
            case PropertyInfo property:
            {
                Type = property.PropertyType;
                _setValue = property.SetValue;
                break;
            }
            case FieldInfo field:
            {
                Type = field.FieldType;
                _setValue = field.SetValue;
                break;
            }
            default:
                throw new NotSupportedException($"Member type {memberInfo.GetType()} is not supported.");
        }
        
    }

    public void SetValue(object instance, object value) => _setValue(instance, value);
}