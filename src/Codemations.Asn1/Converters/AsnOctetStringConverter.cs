using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

internal class AsnOctetStringConverter : AsnConverter<byte[]>
{
    public override object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        return reader.ReadOctetString(tag);
    }

    public override void Write(AsnWriter writer, Asn1Tag? tag, byte[] value, AsnSerializer serializer)
    {
        writer.WriteOctetString(value, tag);
    }
}