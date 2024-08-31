using System.Numerics;
using Codemations.Asn1.Attributes;

namespace Codemations.Asn1.Tests;

public class SequenceElement
{
    [AsnTag(0x81)]
    [AsnOptional]
    public byte[]? OctetString { get; set; }

    [AsnTag(0x82)]
    public BigInteger? Integer { get; set; }
}
