namespace Shaml.Reflections
{
	[Flags]
	public enum ShamlTypeCode : ulong
	{
		Byte = 1 << 1,
		Short = 1 << 2,
		UShort = 1 << 3,
		Int = 1 << 4,
		UInt = 1 << 5,
		Long = 1 << 6,
		ULong = 1 << 7,
		Int128 = 1 << 8,
		UInt128 = 1 << 9,
		Float = 1 << 10,
		Double = 1 << 11,
		Decimal = 1 << 12,
		Bool = 1 << 13,
		DateTime = 1 << 14,
		Char = 1 << 15,
		String = 1 << 16,

		List = 1 << 17,
		Dictionary = 1 << 18,
		
		Object = 1 << 30,
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
				
				_ => ShamlTypeCode.Object
			};
		}
	}
}