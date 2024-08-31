using Codemations.Asn1.Attributes;

namespace Codemations.Asn1.Tests;

[AsnChoice]
public class InvalidChoiceElement
{
    [AsnTag(0x80)]
    public int? IntegerElement { get; set; }

    [AsnTag(0x80)]
    public bool? BooleanElement { get; set; }
}
