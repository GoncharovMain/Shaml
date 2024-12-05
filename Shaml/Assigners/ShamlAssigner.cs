using Shaml.Reflections;
using Shaml.Tokens;

namespace Shaml.Assigners;

public class ShamlAssigner<TInstance>
{
    private readonly Node _node;
    private readonly Type _type;
		
    private readonly IAssigner _assigner;

    public ShamlAssigner(ReadOnlyMemory<char> buffer)
    {
        _node = ShamlConverter.Parse(buffer);
        _type = typeof(TInstance);

        ShamlTypeCode typeCode = _type.ToTypeCode();
			
        switch (typeCode)
        {
            case ShamlTypeCode.List:
                _assigner = new ListAssigner(_type, _node.Children);
                break;
            case ShamlTypeCode.Dictionary:
                _assigner = new DictionaryAssigner(_type, _node.Children);
                break;
            case ShamlTypeCode.Array:
                _assigner = new ArrayAssigner(_type, _node.Children);
                break;
            case ShamlTypeCode.Object:
                _assigner = new ObjectAssigner(_type, _node.Children);
                break;
            default:
                throw new NotImplementedException($"Not implemented for {_type}");
        }
    }
		
    public TInstance Assign(TInstance instance)
    {
        instance ??= Activator.CreateInstance<TInstance>();
			
        object boxedInstance = instance;
			
        _assigner.Assign(ref boxedInstance);

        return instance;
    }
}