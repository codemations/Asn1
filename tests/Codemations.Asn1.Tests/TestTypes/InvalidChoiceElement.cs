namespace Codemations.Asn1.Tests
{
    [AsnChoice]
    public class InvalidChoiceElement
    {
        [AsnElement(0x80)]
        public int? IntegerElement { get; set; }

        [AsnElement(0x80)]
        public bool? BooleanElement { get; set; }
    }
}
