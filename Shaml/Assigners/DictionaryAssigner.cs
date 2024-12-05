using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Shaml.Reflections;
using Shaml.Tokens;

namespace Shaml.Assigners;

public class DictionaryAssigner : IAssigner
{
    private object _instance;
    
    private readonly Type _type;
    
    private readonly Type _keyType;
    private readonly Type _valueType;

    private readonly MethodInfo _method_add;
    
    private readonly Dictionary<StaticReference, Token> _tokens;
    private readonly Dictionary<StaticReference, IAssigner> _keyAssigners;
    private readonly Dictionary<StaticReference, IAssigner> _valueAssigners;
    
    public DictionaryAssigner(Type type, Dictionary<IReference, Token> tokens)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(tokens);
        
        _type = type;
        _method_add = _type.GetMethod("Add");
        
        _tokens = new Dictionary<StaticReference, Token>(Assigner.Comparer);
        _keyAssigners = new Dictionary<StaticReference, IAssigner>(Assigner.Comparer);
        _valueAssigners = new Dictionary<StaticReference, IAssigner>(Assigner.Comparer);
        
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
                    _valueAssigners.Add(staticReference, valueAssigner);
                    
                    _tokens.Add(staticReference, token);
                    break;
                
                default:
                    continue;
            }
        }
    }
    
    public void Assign([NotNull] ref object dictionary)
    {
        dictionary ??= Activator.CreateInstance(_type, _tokens.Count);
        
        foreach ((StaticReference reference, Token token) in _tokens)
        {
            IAssigner keyAssigner = _keyAssigners[reference];
            IAssigner valueAssigner = _valueAssigners[reference];
            
            object keyInstance = reference.CreateInstance(_keyType);
            object valueInstance = token.CreateInstance(_valueType);
            
            keyAssigner.Assign(ref keyInstance);
            valueAssigner.Assign(ref valueInstance);
            
            _method_add.Invoke(dictionary, new[] { keyInstance, valueInstance });
        }
        
        /// Save in cache.
        _instance = dictionary;
    }
}