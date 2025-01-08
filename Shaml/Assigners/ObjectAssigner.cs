using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Shaml.Reflections;
using Shaml.Tokens;

namespace Shaml.Assigners;

internal sealed class ObjectAssigner : Assigner, IAssigner
{
    public ObjectAssigner(Type type, Dictionary<IReference, Token> tokens)
    {
        _type = type;
        
        Cache = new Cache()
        {
            Type = _type
        };
        
        _assigners = new Dictionary<IReference, IAssigner>(Assigner.Comparer);

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

                    _assigners.Add(staticReference,
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

    public override void Assign([NotNull] ref object instance)
    {
        foreach (MemberAssignerWrapper memberAssignerWrapper in _assigners.Values)
        {
            memberAssignerWrapper.Assign(ref instance);
        }
    }

    public override void InitializeContext(string pathRoot, Dictionary<string, Cache> globalContext)
    {
        globalContext.Add(pathRoot, Cache);
        
        foreach ((IReference reference, IAssigner memberAssignerWrapper) in _assigners)
        {
            string path = pathRoot + Dot + reference.Literal;
            
            memberAssignerWrapper.InitializeContext(path, globalContext);
        }
    }
    
    private sealed class MemberAssignerWrapper : IAssigner
    {
        private readonly Token _token;
        private readonly GetterInstance _getterInstance;
        private readonly MemberAssigner _memberAssigner;
        private readonly IAssigner _assigner;
        private object _parentInstance;
        public Cache Cache { get; private init; }
        public MemberAssignerWrapper(Token token, IAssigner assigner,
            MemberAssigner memberAssigner)
        {
            _token = token;
            _assigner = assigner;

            _memberAssigner = memberAssigner;

            Cache = new Cache()
            {
                Type = _memberAssigner.Type
            };
            
            if (_memberAssigner.IsScalar)
            {
                _getterInstance = GetCache;
                return;
            }

            _getterInstance = GetParentInstance;
        }
        private object GetParentInstance() => _memberAssigner.GetValue(_parentInstance);
        private object GetCache() => _assigner.Cache.Instance;

        /// <summary>
        /// This method has specified logic of an assignment members of object.
        /// First, method <see cref="MemberAssignerWrapper.Assign"/> and
        /// <see cref="ObjectAssigner.Assign"/> get the same object.
        /// </summary>
        public void Assign([NotNull] ref object parentInstance)
        {
            _parentInstance = parentInstance;
            
            object memberInstance = _getterInstance() ?? GetCache();
            
            _assigner.Assign(ref memberInstance);
            
            _memberAssigner.SetValue(parentInstance, memberInstance);
        }

        public void InitializeContext(string pathRoot, Dictionary<string, Cache> globalContext)
        {
            _assigner.InitializeContext(pathRoot, globalContext);
        }
    }
}