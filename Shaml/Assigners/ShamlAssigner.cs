using Shaml.Reflections;
using Shaml.Tokens;

namespace Shaml.Assigners;

public class ShamlAssigner<TInstance> where TInstance : notnull
{
    private readonly Node _nodeRoot;
    private readonly Type _type;

    private readonly IAssigner _assignerRoot;
    
    private Dictionary<string, Cache> _globalContext = new();
    public ShamlAssigner(ReadOnlyMemory<char> buffer)
    {
        _globalContext = new();

        _nodeRoot = ShamlConverter.Parse(buffer, _globalContext);
        _type = typeof(TInstance);
        
        
        Dictionary<string, Token> stack = new();
        
        Dictionary<string, Token> deferred = _nodeRoot.GetKeys();
        
        string[] keys = deferred.Keys.ToArray();

        foreach ((string reference, Token token) in deferred)
        {
            switch (token)
            {
                case Node node:
                    if (node.References.Length is 0)
                    {
                        stack.Add(reference, node);
                        deferred.Remove(reference);
                    }
                    break;
                
                case Scalar scalar:
                    if (scalar.Marks.Any(mark => mark.Type is MarkType.Reference))
                    {
                        continue;
                    }
                    stack.Add(reference, scalar);
                    deferred.Remove(reference);
                    break;
                
                case CompositeScalar compositeScalar:
                    if (compositeScalar.Marks.Any(mark => mark.Type is MarkType.Reference))
                    {
                        continue;
                    }
                    stack.Add(reference, compositeScalar);
                    deferred.Remove(reference);
                    break;
            }
        }


        while (deferred.Count > 0)
        {
            foreach ((string reference, Token token) in deferred)
            {
                Mark[] referencesMarks;

                switch (token)
                {
                    case Node node:
                        referencesMarks = node.References;
                        break;
                    
                    case Scalar scalar:
                        referencesMarks = scalar.Marks.Where(mark => mark.Type is MarkType.Reference).ToArray();
                        break;
                    
                    case CompositeScalar compositeScalar:
                        referencesMarks = compositeScalar.Marks.Where(mark => mark.Type is MarkType.Reference).ToArray();
                        break;
                    
                    default:
                        continue;
                }
                
                string[] references = referencesMarks
                    .Select(mark => buffer.Span.Slice(mark.Start + 2, mark.Length - 3).ToString())
                    // Before build stack, check for contains references.
                    .Where(referenceInner => keys.Contains(referenceInner))
                    .ToArray();
                
                if (references.All(innerReference => stack.ContainsKey(innerReference)))
                {
                    stack.Add(reference, token);
                    deferred.Remove(reference);
                }
            }
        }
        
        ShamlTypeCode typeCode = _type.ToTypeCode();

        switch (typeCode)
        {
            case ShamlTypeCode.List:
                _assignerRoot = new ListAssigner(_type, _nodeRoot.Children);
                break;
            case ShamlTypeCode.Dictionary:
                _assignerRoot = new DictionaryAssigner(_type, _nodeRoot.Children);
                break;
            case ShamlTypeCode.Array:
                _assignerRoot = new ArrayAssigner(_type, _nodeRoot.Children);
                break;
            case ShamlTypeCode.Object:
                _assignerRoot = new ObjectAssigner(_type, _nodeRoot.Children);
                break;
            default:
                throw new NotImplementedException($"Not implemented for {_type}");
        }
        
        (_assignerRoot as Assigner).InitializeContext(_globalContext);

        foreach ((string reference, Token token) in stack)
        {
            if (_globalContext.TryGetValue(reference, out Cache cache))
            {
                if ((cache.Type.ToTypeCode() & ShamlTypeCode.Scalar) != 0)
                {
                    cache.Instance = token.CreateInstance(cache.Type);
                    continue;
                }
                
                cache.Instance ??= token.CreateInstance(cache.Type);
            }
        }
    }

    public TInstance Assign(TInstance instance)
    {
        instance ??= Activator.CreateInstance<TInstance>();

        object boxedInstance = instance;
        
        _assignerRoot.Assign(ref boxedInstance);

        return instance;
    }
}