namespace Shaml.Reflections
{
	public class ReflectionAssigner
	{
		private const ulong _numberPrimitiveTypes = (ulong)(
			PrimitiveType.UShort |
			PrimitiveType.Short |
			PrimitiveType.UInt |
			PrimitiveType.Int |
			PrimitiveType.ULong |
			PrimitiveType.Long |
			PrimitiveType.Float |
			PrimitiveType.Double |
			PrimitiveType.Decimal);

		private const ulong _hasTryParse = _numberPrimitiveTypes | (ulong)(
			PrimitiveType.Bool |
			PrimitiveType.DateTime |
			PrimitiveType.Char);

		private const ulong _isInteger = (ulong)(
			PrimitiveType.Byte |
			PrimitiveType.UShort |
			PrimitiveType.Short |
			PrimitiveType.UInt |
			PrimitiveType.Int |
			PrimitiveType.ULong |
			PrimitiveType.Long);

		private readonly object _instance;
		private readonly Type _type;
		private readonly GetterDelegate _getValue;
		private readonly SetterDelegate _setValue;

		public Type MemberType { get; }
		
		public readonly bool IsNumber;
		public readonly bool IsInteger;

		public readonly PrimitiveType PrimitiveType;
		
		public ReflectionAssigner(object instance, Type memberType, GetterDelegate getValue, SetterDelegate setValue)
		{
			_instance = instance;
			_type = instance.GetType();
			_getValue = getValue;
			_setValue = setValue;

			MemberType = memberType;

			PrimitiveType primitiveType = memberType.ToPrimitiveType();
			
			PrimitiveType = primitiveType;

			IsNumber = (_numberPrimitiveTypes & (ulong)primitiveType) != 0;
			IsInteger = (_isInteger & (ulong)primitiveType) != 0;
		}

		public void SetValue(object value)
		{
			_setValue(_instance, value);
		}

		public object GetValue()
		{
			return _getValue(_instance);
		}

		public bool IsContainsInstance
		{
			get
			{
				if (_type.IsTypeDefinition)
				{
					return false;
				}

				return GetValue() != null;
			}
		}
	}
}