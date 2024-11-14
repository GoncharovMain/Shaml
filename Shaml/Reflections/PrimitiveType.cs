namespace Shaml.Reflections
{
	[Flags]
	public enum PrimitiveType : ulong
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

		Object = 1 << 17,
	}

	public static class PrimitiveTypeExtensions
	{
		public static PrimitiveType ToPrimitiveType(this Type type)
		{
			return type switch
			{
				not null when type == typeof(String)	=> PrimitiveType.String,
				
				not null when type == typeof(Byte)		=> PrimitiveType.Byte,
				not null when type == typeof(Int16)		=> PrimitiveType.Short,
				not null when type == typeof(UInt16)	=> PrimitiveType.UShort,
				not null when type == typeof(Int32)		=> PrimitiveType.Int,
				not null when type == typeof(UInt32)	=> PrimitiveType.UInt,
				not null when type == typeof(Int64)		=> PrimitiveType.Long,
				not null when type == typeof(UInt64)	=> PrimitiveType.ULong,
				not null when type == typeof(Int128)	=> PrimitiveType.Int128,
				not null when type == typeof(UInt128)	=> PrimitiveType.UInt128,
				
				not null when type == typeof(Single)	=> PrimitiveType.Float,
				not null when type == typeof(Double)	=> PrimitiveType.Double,
				not null when type == typeof(Decimal)	=> PrimitiveType.Decimal,
				not null when type == typeof(Boolean)	=> PrimitiveType.Bool,
				not null when type == typeof(DateTime)	=> PrimitiveType.DateTime,
				_ => PrimitiveType.Object
			};
		}
	}
}