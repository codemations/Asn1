using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

internal class AsnBooleanConverter : AsnConverter<bool>
{
    public override object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        return reader.ReadBoolean(tag);
    }

    public override void Write(AsnWriter writer, Asn1Tag? tag, bool value, AsnSerializer serializer)
    {
        writer.WriteBoolean(value, tag);
    }
}