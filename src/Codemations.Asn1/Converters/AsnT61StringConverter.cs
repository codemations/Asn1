using Codemations.Asn1.Extensions;
using Codemations.Asn1.Types;
using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

internal class AsnT61StringConverter : AsnConverter<T61String>
{
    public override object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        return reader.ReadT61String(tag);
    }

    public override void Write(AsnWriter writer, Asn1Tag? tag, T61String value, AsnSerializer serializer)
    {
        writer.WriteT61String(value, tag);
    }
}
