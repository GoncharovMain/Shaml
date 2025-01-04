using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Shaml.Reflections;
using Shaml.Tokens;

namespace Shaml.Assigners;

internal sealed class ObjectAssigner : IAssigner
{
    private readonly Type _type;

    private readonly Dictionary<StaticReference, MemberAssignerWrapper> _assignerWrappers;
    
    public Cache Cache { get; private set; }
    
    public ObjectAssigner(Type type, Dictionary<IReference, Token> tokens)
    {
        Cache = new Cache();
        
        _type = type;
        
        _assignerWrappers = new Dictionary<StaticReference, MemberAssignerWrapper>(Assigner.Comparer);

        foreach ((IReference reference, Token token) in tokens)
        {
            switch (reference)
            {
                case StaticReference staticReference:

                    string memberName = reference.Literal;

                    MemberInfo memberInfo = _type.GetMember(
                        name: memberName,
                        type: Token.SetterMemberTypes,
                        bindingAttr: Token.PublicMembers).FirstOrDefault();

                    if (memberInfo is null)
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
        Cache.Instance = instance;
    }

    public void InitializeContext(string pathRoot, Dictionary<string, Cache> globalContext)
    {
        foreach ((StaticReference reference, MemberAssignerWrapper memberAssignerWrapper) in _assignerWrappers)
        {
            string path = pathRoot + Assigner.Dot + reference.Literal;
            
            memberAssignerWrapper.InitializeContext(path, globalContext);
        }
    }

    public T ToObject<T>() => (T)Cache.Instance;

    private class MemberAssignerWrapper : IAssigner
    {
        private readonly Token _token;
        private readonly GetterInstance _getterInstance;
        private readonly MemberAssigner _memberAssigner;
        private readonly IAssigner _assigner;
        public Cache Cache { get; private set; }
        public MemberAssignerWrapper(Token token, IAssigner assigner,
            MemberAssigner memberAssigner)
        {
            Cache = new Cache();
            _token = token;
            _assigner = assigner;

            _memberAssigner = memberAssigner;

            if (_memberAssigner.IsScalar)
            {
                _getterInstance = CreateInstance;
                return;
            }

            _getterInstance = GetInstance;
        }
        private object GetInstance() => _memberAssigner.GetValue(Cache.Instance);
        private object CreateInstance() => _token.CreateInstance(_memberAssigner.Type);

        public void Assign([NotNull] ref object instance)
        {
            object memberInstance = _memberAssigner.GetValue(instance) ?? CreateInstance();

            Cache.Instance = memberInstance;

            _assigner.Assign(ref memberInstance);

            _memberAssigner.SetValue(instance, memberInstance);
        }

        public void InitializeContext(string pathRoot, Dictionary<string, Cache> globalContext)
        {
            globalContext[pathRoot] = Cache;
            
            _assigner.InitializeContext(pathRoot, globalContext);
        }
    }
}