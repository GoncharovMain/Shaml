using System.Diagnostics.CodeAnalysis;
using Shaml.Extension;
using Shaml.Reflections;
using Shaml.Tokens;

namespace Shaml.Assigners;

internal abstract class Assigner : IAssigner
{
    protected Type _type;
    protected Dictionary<IReference, IAssigner> _assigners;
    
    public const char Dot = '.';
    
    /// <summary>
    /// Is a cache for an instance of type <see cref="_type"/>
    /// </summary>
    public Cache Cache { get; protected set; }

    public void InitializeContext(Dictionary<string, Cache> globalContext)
    {
        foreach ((IReference reference, IAssigner assigner) in _assigners)
        {
            assigner.InitializeContext(reference.Literal, globalContext);
        }
    }
    public virtual void InitializeContext(string pathRoot, Dictionary<string, Cache> globalContext)
    {
        globalContext[pathRoot] = Cache;
    }

    public T ToObject<T>() => (T)Cache.Instance;
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
            
            case ({ IsScalar: true }, CompositeScalar scalar):
                return new CompositeAssigner(type, scalar);

            default:
                throw new NotImplementedException($"Not implemented for {type} and {token.Type}");
        }
    }
    public abstract void Assign([NotNull] ref object instance);
}