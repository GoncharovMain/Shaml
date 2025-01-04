using System.Reflection;
using Shaml.Extension;
using Shaml.Reflections;

namespace Shaml.Assigners;

internal sealed class MemberAssigner
{
    private readonly GetterDelegate _getValue;
    private readonly SetterDelegate _setValue;
    public Type Type { get; }
    public readonly bool IsScalar;
    
    public MemberAssigner(MemberInfo memberInfo)
    {
        switch (memberInfo)
        {
            case PropertyInfo property:
            {
                Type = property.PropertyType;
                _getValue = property.GetValue;
                _setValue = property.SetValue;
                break;
            }
            case FieldInfo field:
            {
                Type = field.FieldType;
                _getValue = field.GetValue;
                _setValue = field.SetValue;
                break;
            }
            default:
                throw new NotSupportedException($"Member type {memberInfo.GetType()} is not supported.");
        }
        
        IsScalar = (Type.ToTypeCode() & ShamlTypeCode.Scalar) != 0;
    }

    public object GetValue(object instance) => _getValue(instance);
    public void SetValue(object instance, object value) => _setValue(instance, value);
}