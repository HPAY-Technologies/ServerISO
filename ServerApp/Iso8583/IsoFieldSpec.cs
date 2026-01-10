namespace Iso8583
{
    public class IsoFieldSpec
    {
        public int FieldNumber { get; }
        public FieldType Type { get; }
        public int Length { get; }

        public IsoFieldSpec(int fieldNumber, FieldType type, int length)
        {
            FieldNumber = fieldNumber;
            Type = type;
            Length = length;
        }
    }
}
