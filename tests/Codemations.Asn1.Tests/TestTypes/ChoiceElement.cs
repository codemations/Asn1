using Codemations.Asn1.Attributes;

namespace Codemations.Asn1.Tests
{
    [AsnChoice]
    public class ChoiceElement
    {
        [AsnTag(0xA0)]
        public SequenceElement? SequenceElement { get; set; }

        [AsnTag(0x81)]
        public bool? BooleanElement { get; set; }
    }
}
