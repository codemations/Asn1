using Codemations.Asn1.Extensions;
using Codemations.Asn1.Types;
using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

internal class AsnBmpStringConverter : AsnConverter<BmpString>
{
    public override object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        return reader.ReadBmpString(tag);
    }

    public override void Write(AsnWriter writer, Asn1Tag? tag, BmpString value, AsnSerializer serializer)
    {
        writer.WriteBmpString(value, tag);
    }
}
