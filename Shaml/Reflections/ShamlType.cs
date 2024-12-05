namespace Shaml.Reflections
{
	public class ShamlType
	{
		public Type MemberType { get; }
		public ShamlTypeCode TypeCode { get; }

		public readonly bool IsNumber;
		public readonly bool IsInteger;
		public readonly bool IsScalar;
		public ShamlType(Type memberType)
		{
			MemberType = memberType;
			TypeCode = memberType.ToTypeCode();

			IsNumber = (ShamlTypeCode.Integer & TypeCode) != 0;
			IsInteger = (ShamlTypeCode.Number & TypeCode) != 0;
			IsScalar = (ShamlTypeCode.Scalar & TypeCode) != 0;
		}
	}
}