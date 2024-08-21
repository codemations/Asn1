using Codemations.Asn1.Extensions;
using Codemations.Asn1.Types;
using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

internal class AsnIA5StringConverter : AsnConverter<AsnIA5String>
{
    public override object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        return reader.ReadIA5String(tag);
    }

    public override void Write(AsnWriter writer, Asn1Tag? tag, AsnIA5String value, AsnSerializer serializer)
    {
        writer.WriteIA5String(value, tag);
    }
}
