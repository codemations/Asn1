using Codemations.Asn1.Extensions;
using Codemations.Asn1.Types;
using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

internal class AsnNumericStringConverter : AsnConverter<AsnNumericString>
{
    protected override AsnNumericString ReadTyped(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        return reader.ReadNumericString(tag);
    }

    protected override void WriteTyped(AsnWriter writer, Asn1Tag? tag, AsnNumericString value, AsnSerializer serializer)
    {
        writer.WriteNumericString(value, tag);
    }
}
