using Codemations.Asn1.Types;
using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

internal class AsnT61StringConverter : AsnConverter<AsnT61String>
{
    protected override AsnT61String ReadTyped(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        return reader.ReadT61String(tag);
    }

    protected override void WriteTyped(AsnWriter writer, Asn1Tag? tag, AsnT61String value, AsnSerializer serializer)
    {
        writer.WriteT61String(value, tag);
    }
}
