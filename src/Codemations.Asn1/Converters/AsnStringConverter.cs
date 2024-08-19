using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

internal class AsnStringConverter : AsnConverter<string>
{
    protected override string ReadTyped(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        return reader.ReadCharacterString(UniversalTagNumber.IA5String, tag);
    }

    protected override void WriteTyped(AsnWriter writer, Asn1Tag? tag, string value, AsnSerializer serializer)
    {
        writer.WriteCharacterString(UniversalTagNumber.IA5String, value, tag);
    }
}
