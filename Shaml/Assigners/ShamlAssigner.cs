using Shaml.Models;
using Shaml.Reflections;
using Shaml.Tokens;

namespace Shaml.Assigners;

public class ShamlAssigner<TInstance>
{
    private readonly Node _nodeRoot;
    private readonly Type _type;

    private readonly IAssigner _assignerRoot;

    private readonly IAssigner _immediateAssigner;
    private readonly IAssigner _deferredAssigner;

    private Dictionary<string, Cache> _globalContext = new();
    public ShamlAssigner(ReadOnlyMemory<char> buffer)
    {
        _globalContext = new();

        _nodeRoot = ShamlConverter.Parse(buffer, _globalContext);
        _type = typeof(TInstance);

        ShamlTypeCode typeCode = _type.ToTypeCode();

        Node immediate = _nodeRoot.FilterByImmediate();
        Node deferred = _nodeRoot.FilterByDeferred();

        switch (typeCode)
        {
            case ShamlTypeCode.List:
                _assignerRoot = new ListAssigner(_type, _nodeRoot.Children);
                
                _immediateAssigner = new ListAssigner(_type, immediate.Children);
                _deferredAssigner = new ListAssigner(_type, deferred.Children);
                break;
            case ShamlTypeCode.Dictionary:
                _assignerRoot = new DictionaryAssigner(_type, _nodeRoot.Children);
                
                _immediateAssigner = new DictionaryAssigner(_type, immediate.Children);
                _deferredAssigner = new DictionaryAssigner(_type, deferred.Children);
                break;
            case ShamlTypeCode.Array:
                _assignerRoot = new ArrayAssigner(_type, _nodeRoot.Children);
                
                _immediateAssigner = new ArrayAssigner(_type, immediate.Children);
                _deferredAssigner = new ArrayAssigner(_type, deferred.Children);
                break;
            case ShamlTypeCode.Object:
                _assignerRoot = new ObjectAssigner(_type, _nodeRoot.Children);
                
                _immediateAssigner = new ObjectAssigner(_type, immediate.Children);
                _deferredAssigner = new ObjectAssigner(_type, deferred.Children);
                break;
            default:
                throw new NotImplementedException($"Not implemented for {_type}");
        }
        
        _assignerRoot.InitializeContext(String.Empty, _globalContext);
    }

    public TInstance Assign(TInstance instance)
    {
        instance ??= Activator.CreateInstance<TInstance>();

        object boxedInstance = instance;
        
        _assignerRoot.Assign(ref boxedInstance);

        return instance;
    }
}