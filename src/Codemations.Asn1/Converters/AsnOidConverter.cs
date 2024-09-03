using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

internal class AsnOidConverter : AsnConverter<string>
{
    public override object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        return reader.ReadObjectIdentifier(tag);
    }

    public override void Write(AsnWriter writer, Asn1Tag? tag, string value, AsnSerializer serializer)
    {
        writer.WriteObjectIdentifier(value, tag);
    }
}