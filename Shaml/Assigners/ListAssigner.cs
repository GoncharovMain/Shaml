using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Shaml.Reflections;
using Shaml.Tokens;

namespace Shaml.Assigners;

public class ListAssigner : IAssigner
{
    // cache
    private object _instance;
    private readonly Type _type;
    private readonly Type _itemType;
    private readonly MethodInfo _method_add;
    
    private readonly Dictionary<IndexReference, Token> _tokens;
    private readonly Dictionary<IndexReference, IAssigner> _assigners;
    
    public ListAssigner(Type type, Dictionary<IReference, Token> listTokens)
    {
        _type = type;
        _method_add = _type.GetMethod("Add");
        _itemType = _type.GetGenericArguments()[0];
        
        _tokens = new(Assigner.Comparer);
        _assigners = new(Assigner.Comparer);
        
        foreach ((IReference reference, Token token) in listTokens)
        {
            IndexReference indexReference = (IndexReference)reference;
            
            IAssigner assigner = Assigner.Create(_itemType, token);
            
            _tokens.Add(indexReference, token);
            _assigners.Add(indexReference, assigner);
        }

        _instance = null;
    }

    public void Assign([NotNull] ref object list)
    {
        list ??= Activator.CreateInstance(_type, _tokens.Count);

        foreach ((IndexReference reference, Token token) in _tokens)
        {
            object item = token.CreateInstance(_itemType);
            
            IAssigner assigner = _assigners[reference];

            assigner.Assign(ref item);
            
            _method_add.Invoke(list, new[] { item });
        }
        
        /// Save in cache.
        _instance = list;
    }
}