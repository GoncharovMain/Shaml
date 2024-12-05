namespace Shaml.Reflections
{
	[Flags]
	public enum ShamlTypeCode : ulong
	{
		Byte = 1UL << 1,
		Short = 1UL << 2,
		UShort = 1UL << 3,
		Int = 1UL << 4,
		UInt = 1UL << 5,
		Long = 1UL << 6,
		ULong = 1UL << 7,
		Int128 = 1UL << 8,
		UInt128 = 1UL << 9,
		Float = 1UL << 10,
		Double = 1UL << 11,
		Decimal = 1UL << 12,
		Bool = 1UL << 13,
		DateTime = 1UL << 14,
		Char = 1UL << 15,
		String = 1UL << 16,
		
		Enum = 1UL << 17,

		List = 1UL << 18,
		Dictionary = 1UL << 19,
		Array = 1UL << 20,
		
		Object = 1UL << 30,
		
		Integer = Byte | UShort | Short | UInt | Int | ULong | Long,
		Number = Integer | Float | Double | Decimal,
		Scalar =  Number | Char | String | Bool | DateTime | Enum,
		
		Node = List | Dictionary | Array | Object,
	}

	public static class TypeCodeExtensions
	{
		public static ShamlTypeCode ToTypeCode(this Type type)
		{
			return type switch
			{
				not null when type == typeof(String) => ShamlTypeCode.String,
				not null when type == typeof(Char) => ShamlTypeCode.Char,
				not null when type == typeof(Byte) => ShamlTypeCode.Byte,
				not null when type == typeof(Int16) => ShamlTypeCode.Short,
				not null when type == typeof(UInt16) => ShamlTypeCode.UShort,
				not null when type == typeof(Int32) => ShamlTypeCode.Int,
				not null when type == typeof(UInt32) => ShamlTypeCode.UInt,
				not null when type == typeof(Int64) => ShamlTypeCode.Long,
				not null when type == typeof(UInt64) => ShamlTypeCode.ULong,
				not null when type == typeof(Int128) => ShamlTypeCode.Int128,
				not null when type == typeof(UInt128) => ShamlTypeCode.UInt128,
				not null when type == typeof(Single) => ShamlTypeCode.Float,
				not null when type == typeof(Double) => ShamlTypeCode.Double,
				not null when type == typeof(Decimal) => ShamlTypeCode.Decimal,
				not null when type == typeof(Boolean) => ShamlTypeCode.Bool,
				not null when type == typeof(DateTime) => ShamlTypeCode.DateTime,
				
				{ IsGenericType: true } when type.GetGenericTypeDefinition() == typeof(List<>)
					=> ShamlTypeCode.List,
				
				{ IsGenericType: true } when type.GetGenericTypeDefinition() == typeof(Dictionary<,>)
					=> ShamlTypeCode.Dictionary,
				{ IsArray: true } => ShamlTypeCode.Array,
				
				_ => ShamlTypeCode.Object
			};
		}
	}
}