using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

internal class AsnOidConverter : AsnConverter<Oid>
{
    public override object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        var oidStr = reader.ReadObjectIdentifier(tag);
        return new Oid(oidStr, false);
    }

    public override void Write(AsnWriter writer, Asn1Tag? tag, Oid value, AsnSerializer serializer)
    {
        writer.WriteObjectIdentifier((string)value, tag);
    }
}