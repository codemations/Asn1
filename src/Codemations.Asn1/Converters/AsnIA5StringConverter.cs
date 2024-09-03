using Codemations.Asn1.Extensions;
using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

public class AsnIA5StringConverter : AsnConverter<string>
{
    public override object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        return reader.ReadIA5String(tag);
    }

    public override void Write(AsnWriter writer, Asn1Tag? tag, string value, AsnSerializer serializer)
    {
        writer.WriteIA5String(value, tag);
    }
}
