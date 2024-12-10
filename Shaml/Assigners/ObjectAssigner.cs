using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Shaml.Reflections;
using Shaml.Tokens;

namespace Shaml.Assigners;

public sealed class ObjectAssigner : IAssigner
{
    private object _instance;
    private readonly Type _type;
    
    private readonly Dictionary<StaticReference, MemberAssignerWrapper> _assignerWrappers;
    
    public ObjectAssigner(Type type, Dictionary<IReference, Token> tokens)
    {
        _instance = null;
        _type = type;
        
        _assignerWrappers = new Dictionary<StaticReference, MemberAssignerWrapper>(Assigner.Comparer);
        
        foreach ((IReference reference, Token token) in tokens)
        {
            switch (reference)
            {
                case StaticReference staticReference:
                    
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
                    
                    IAssigner assigner = Assigner.Create(memberAssigner.Type, token);
                    
                    _assignerWrappers.Add(staticReference, 
                        new MemberAssignerWrapper(token, assigner, memberAssigner));
                    
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
        foreach (MemberAssignerWrapper memberAssignerWrapper in _assignerWrappers.Values)
        {
            memberAssignerWrapper.Assign(ref instance);
        }

        /// Save in cache.
        _instance = instance;
    }

    public T ToObject<T>() => (T)_instance;
    
    private class MemberAssignerWrapper : IAssigner
    {
        private object _instance;
        private readonly Token _token;
        public MemberAssigner MemberAssigner { get; }
        public IAssigner Assigner { get; }
        public GetterInstance GetterInstance { get; }

        public MemberAssignerWrapper(Token token, IAssigner assigner, MemberAssigner memberAssigner)
        {
            _instance = null;
            _token = token;
            Assigner = assigner;
            
            MemberAssigner = memberAssigner;

            if (MemberAssigner.IsScalar)
            {
                GetterInstance = CreateInstance;
                return;
            }
            
            GetterInstance = GetInstance;
        }
        private object GetInstance() => MemberAssigner.GetValue(_instance);
        private object CreateInstance() => _token.CreateInstance(MemberAssigner.Type);

        public void Assign([NotNull] ref object instance)
        {
            _instance = instance;
            
            object memberInstance = GetterInstance() ?? CreateInstance();

            Assigner.Assign(ref memberInstance);

            MemberAssigner.SetValue(instance, memberInstance);
        }
    }
}