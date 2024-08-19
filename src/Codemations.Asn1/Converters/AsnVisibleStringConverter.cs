using Codemations.Asn1.Types;
using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

internal class AsnVisibleStringConverter : AsnConverter<AsnVisibleString>
{
    protected override AsnVisibleString ReadTyped(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        return reader.ReadVisibleString(tag);
    }

    protected override void WriteTyped(AsnWriter writer, Asn1Tag? tag, AsnVisibleString value, AsnSerializer serializer)
    {
        writer.WriteVisibleString(value, tag);
    }
}
