using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

public class AsnUtf8StringConverter : AsnConverter<string>
{
    public override object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        return reader.ReadCharacterString(UniversalTagNumber.UTF8String, tag);
    }

    public override void Write(AsnWriter writer, Asn1Tag? tag, string value, AsnSerializer serializer)
    {
        writer.WriteCharacterString(UniversalTagNumber.UTF8String, value, tag);
    }
}
