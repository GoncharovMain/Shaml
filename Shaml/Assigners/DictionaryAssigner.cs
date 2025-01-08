using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Shaml.Tokens;

namespace Shaml.Assigners;

internal sealed class DictionaryAssigner : Assigner, IAssigner
{
    private readonly Type _keyType;
    private readonly Type _valueType;

    private readonly MethodInfo _method_setItem;

    private readonly Dictionary<StaticReference, Token> _tokens;
    private readonly Dictionary<IReference, IAssigner> _keyAssigners;
    
    public DictionaryAssigner(Type type, Dictionary<IReference, Token> tokens)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(tokens);
     
        _type = type;
        
        _method_setItem = _type.GetMethod("set_Item");

        _tokens = new Dictionary<StaticReference, Token>(Assigner.Comparer);
        _keyAssigners = new Dictionary<IReference, IAssigner>(Assigner.Comparer);
        _assigners = new Dictionary<IReference, IAssigner>(Assigner.Comparer);

        Type[] argumentsGenericTypes = _type.GetGenericArguments();

        _keyType = argumentsGenericTypes[0];
        _valueType = argumentsGenericTypes[1];

        foreach ((IReference reference, Token token) in tokens)
        {
            switch (reference)
            {
                case StaticReference staticReference:

                    IAssigner keyAssigner = Assigner.Create(_keyType, staticReference);
                    IAssigner valueAssigner = Assigner.Create(_valueType, token);

                    _keyAssigners.Add(staticReference, keyAssigner);
                    _assigners.Add(staticReference, valueAssigner);
                    
                    _tokens.Add(staticReference, token);
                    break;

                default:
                    continue;
            }
        }
        
        Cache = new Cache()
        {
            Type = type
        };
    }

    public override void Assign([NotNull] ref object dictionary)
    {
        dictionary ??= Activator.CreateInstance(_type, _tokens.Count);

        foreach ((IReference reference, IAssigner assigner) in _assigners)
        {
            IAssigner keyAssigner = _keyAssigners[reference];
            
            object keyInstance = (reference as StaticReference).CreateInstance(_keyType);
            object valueInstance = assigner.Cache.Instance;
            
            keyAssigner.Assign(ref keyInstance);
            
            _method_setItem.Invoke(dictionary, new[] { keyInstance, valueInstance });
        }
    }

    public override void InitializeContext(string pathRoot, Dictionary<string, Cache> globalContext)
    {
        globalContext.Add(pathRoot, Cache);
        
        foreach ((IReference reference, IAssigner assigner) in _assigners)
        {
            string path = pathRoot + Dot + reference.Literal;

            assigner.InitializeContext(path, globalContext);
        }
    }
}