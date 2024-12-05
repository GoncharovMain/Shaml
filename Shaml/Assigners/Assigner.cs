using System.Diagnostics.CodeAnalysis;
using Shaml.Extension;
using Shaml.Reflections;
using Shaml.Tokens;

namespace Shaml.Assigners;

public abstract class Assigner
{
    /// <summary>
    /// Is a cache for an instance of type <see cref="_type"/>
    /// </summary>
    protected object _instance;

    protected Type _type;
    
    public static ReferenceComparer Comparer { get; } = new();
    public static IAssigner Create(Type type, Token token)
    {
        ShamlType shamlType = type.ToShamlType();
        
        switch (shamlType, token)
        {
            case ({ TypeCode: ShamlTypeCode.List }, Node node):
                return new ListAssigner(type, node.Children);
                
            case ({ TypeCode: ShamlTypeCode.Dictionary }, Node node):
                return new DictionaryAssigner(type, node.Children);
                
            case ({ TypeCode: ShamlTypeCode.Array }, Node node):
                return new ArrayAssigner(type, node.Children);
                
            case ({ TypeCode: ShamlTypeCode.Object }, Node node):
                return new ObjectAssigner(type, node.Children);
                
            case ({ IsScalar: true }, Scalar scalar):
                return new ScalarAssigner(type, scalar);
            
            default:
                throw new NotImplementedException($"Not implemented for {type} and {token.Type}");
        }
    }

    internal abstract void Assign([NotNull] ref object instance);

    public T ToObject<T>() => (T)_instance;
}