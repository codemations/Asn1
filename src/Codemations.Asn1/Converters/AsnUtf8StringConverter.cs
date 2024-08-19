using Codemations.Asn1.Extensions;
using Codemations.Asn1.Types;
using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

internal class AsnUtf8StringConverter : AsnConverter<AsnUtf8String>
{
    protected override AsnUtf8String ReadTyped(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        return reader.ReadUtf8String(tag);
    }

    protected override void WriteTyped(AsnWriter writer, Asn1Tag? tag, AsnUtf8String value, AsnSerializer serializer)
    {
        writer.WriteUtf8String(value, tag);
    }
}
