using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Shaml.Reflections;
using Shaml.Tokens;

namespace Shaml.Assigners;

public sealed class ObjectAssigner : IAssigner
{
    private object _instance;
    private readonly Type _type;
    
    private readonly Dictionary<StaticReference, Token> _tokens;
    private readonly Dictionary<StaticReference, IAssigner> _assigners;
    private readonly Dictionary<StaticReference, MemberAssigner> _memberAssigners;
    
    public ObjectAssigner(Type type, Dictionary<IReference, Token> tokens)
    {
        _type = type;

        _tokens = new Dictionary<StaticReference, Token>(Assigner.Comparer);
        _assigners = new Dictionary<StaticReference, IAssigner>(Assigner.Comparer);
        _memberAssigners = new Dictionary<StaticReference, MemberAssigner>(Assigner.Comparer);

        foreach ((IReference reference, Token token) in tokens)
        {
            switch (reference)
            {
                case StaticReference staticReference:
                    
                    _tokens.Add(staticReference, token);
            
                    string memberName = reference.Key;

                    MemberInfo memberInfo = _type.GetMember(
                        name: memberName,
                        type: Token.SetterMemberTypes,
                        bindingAttr: Token.PublicMembers).FirstOrDefault();
                    
                    if (memberInfo == null)
                    {
                        continue;
                    }

                    MemberAssigner memberAssigner = new(memberInfo);
                        
                    _memberAssigners.Add(staticReference, memberAssigner);

                    IAssigner assigner = Assigner.Create(memberAssigner.Type, token);
                    
                    _assigners.Add(staticReference, assigner);
                    
                    break;
                
                default:
                    /// This version only supports StaticReference keys.
                    /// But in the future, it is necessary to leave
                    /// the possibility to expand and add flexible/dynamic keys.
                    /// Note: when changing the key, dynamically call back
                    /// using an event or attribute.
                    /// 
                    /// Maybe add this feature only for DictionaryAssigner?
                    continue;
            }
        }
    }
    public void Assign([NotNull] ref object instance)
    {
        foreach ((StaticReference reference, IAssigner assigner) in _assigners)
        {
            MemberAssigner memberAssigner = _memberAssigners[reference];
            
            Token token = _tokens[reference];
            
            object memberInstance = memberAssigner.GetValue(instance) ?? 
                                    token.CreateInstance(memberAssigner.Type);

            assigner.Assign(ref memberInstance);
            
            memberAssigner.SetValue(instance, memberInstance);
        }
        
        /// Save in cache.
        _instance = instance;
    }

    public T ToObject<T>() => (T)_instance;
}