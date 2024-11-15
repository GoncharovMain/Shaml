namespace Shaml.Reflections
{
	public class ReflectionAssigner
	{
		private const ulong _numberPrimitiveTypes = (ulong)(
			ShamlTypeCode.UShort |
			ShamlTypeCode.Short |
			ShamlTypeCode.UInt |
			ShamlTypeCode.Int |
			ShamlTypeCode.ULong |
			ShamlTypeCode.Long |
			ShamlTypeCode.Float |
			ShamlTypeCode.Double |
			ShamlTypeCode.Decimal);

		private const ulong _isInteger = (ulong)(
			ShamlTypeCode.Byte |
			ShamlTypeCode.UShort |
			ShamlTypeCode.Short |
			ShamlTypeCode.UInt |
			ShamlTypeCode.Int |
			ShamlTypeCode.ULong |
			ShamlTypeCode.Long);

		private readonly object _instance;
		private readonly Type _type;
		private readonly GetterDelegate _getValue;
		private readonly SetterDelegate _setValue;

		public Type MemberType { get; }
		
		public readonly bool IsNumber;
		public readonly bool IsInteger;
		public readonly ShamlTypeCode TypeCode;
		
		internal ReflectionAssigner(object instance, Type memberType, GetterDelegate getValue, SetterDelegate setValue)
		{
			_instance = instance;
			_type = instance.GetType();
			_getValue = getValue;
			_setValue = setValue;

			MemberType = memberType;

			ShamlTypeCode shamlTypeCode = memberType.ToTypeCode();
			
			TypeCode = shamlTypeCode;

			IsNumber = (_numberPrimitiveTypes & (ulong)shamlTypeCode) != 0;
			IsInteger = (_isInteger & (ulong)shamlTypeCode) != 0;
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