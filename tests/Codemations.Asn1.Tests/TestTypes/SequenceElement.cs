using System.Numerics;

namespace Codemations.Asn1.Tests
{
    [AsnSequence]
    public class SequenceElement
    {
        [AsnElement(0x81, Optional = true)]
        public byte[]? OctetString { get; set; }

        [AsnElement(0x82)]
        public BigInteger? Integer { get; set; }
    }
}
