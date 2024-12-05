using Shaml.Reflections;

namespace Shaml.Extension;

public static class TypeExtensions
{
    public static ShamlType ToShamlType(this Type type)
    {
        return new ShamlType(type);
    }
}