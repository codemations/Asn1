using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

internal class AsnEnumeratedValueConverter : AsnConverter
{
    public override bool CanConvert(Type type)
    {
        return type.IsEnum;
    }

    public override object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        return reader.ReadEnumeratedValue(type, tag);
    }

    public override void Write(AsnWriter writer, Asn1Tag? tag, object value, AsnSerializer serializer)
    {
        writer.WriteEnumeratedValue((Enum)value, tag);
    }
}