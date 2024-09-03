using Codemations.Asn1.Extensions;
using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

public class AsnNumericStringConverter : AsnConverter<string>
{
    public override object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        return reader.ReadNumericString(tag);
    }

    public override void Write(AsnWriter writer, Asn1Tag? tag, string value, AsnSerializer serializer)
    {
        writer.WriteNumericString(value, tag);
    }
}
