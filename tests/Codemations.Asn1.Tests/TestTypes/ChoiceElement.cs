namespace Codemations.Asn1.Tests
{
    [AsnChoice]
    public class ChoiceElement
    {
        [AsnElement(0xA0)]
        public SequenceElement? SequenceElement { get; set; }

        [AsnElement(0x81)]
        public bool? BooleanElement { get; set; }
    }
}
