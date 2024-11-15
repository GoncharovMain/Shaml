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
			switch (type)
			{
				case not null when type == typeof(String):
					return ShamlTypeCode.String;
				case not null when type == typeof(Char):
					return ShamlTypeCode.Char;
				case not null when type == typeof(Byte):
					return ShamlTypeCode.Byte;
				case not null when type == typeof(Int16):
					return ShamlTypeCode.Short;
				case not null when type == typeof(UInt16):
					return ShamlTypeCode.UShort;
				case not null when type == typeof(Int32):
					return ShamlTypeCode.Int;
				case not null when type == typeof(UInt32):
					return ShamlTypeCode.UInt;
				case not null when type == typeof(Int64):
					return ShamlTypeCode.Long;
				case not null when type == typeof(UInt64):
					return ShamlTypeCode.ULong;
				case not null when type == typeof(Int128):
					return ShamlTypeCode.Int128;
				case not null when type == typeof(UInt128):
					return ShamlTypeCode.UInt128;
				case not null when type == typeof(Single):
					return ShamlTypeCode.Float;
				case not null when type == typeof(Double):
					return ShamlTypeCode.Double;
				case not null when type == typeof(Decimal):
					return ShamlTypeCode.Decimal;
				case not null when type == typeof(Boolean):
					return ShamlTypeCode.Bool;
				case not null when type == typeof(DateTime):
					return ShamlTypeCode.DateTime;
				case { IsGenericType: true } when type.GetGenericTypeDefinition() == typeof(List<>):
					return ShamlTypeCode.List;
				case { IsGenericType: true } when type.GetGenericTypeDefinition() == typeof(Dictionary<,>):
					return ShamlTypeCode.Dictionary;
				default:
					return ShamlTypeCode.Object;
			}
		}
	}
}