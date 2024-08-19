using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

internal class AsnBooleanConverter : AsnConverter<bool>
{
    protected override bool ReadTyped(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        return reader.ReadBoolean(tag);
    }

    protected override void WriteTyped(AsnWriter writer, Asn1Tag? tag, bool value, AsnSerializer serializer)
    {
        writer.WriteBoolean(value, tag);
    }
}