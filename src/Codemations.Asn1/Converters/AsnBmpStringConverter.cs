using Codemations.Asn1.Types;
using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

internal class AsnBmpStringConverter : AsnConverter<AsnBmpString>
{
    protected override AsnBmpString ReadTyped(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        return reader.ReadBmpString(tag);
    }

    protected override void WriteTyped(AsnWriter writer, Asn1Tag? tag, AsnBmpString value, AsnSerializer serializer)
    {
        writer.WriteBmpString(value, tag);
    }
}
