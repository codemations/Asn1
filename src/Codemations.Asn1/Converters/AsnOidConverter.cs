using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

internal class AsnOidConverter : AsnConverter<AsnOid>
{
    protected override AsnOid ReadTyped(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        var oidStr = reader.ReadObjectIdentifier(tag);
        return new AsnOid(oidStr, false);
    }

    protected override void WriteTyped(AsnWriter writer, Asn1Tag? tag, AsnOid value, AsnSerializer serializer)
    {
        writer.WriteObjectIdentifier((string)value, tag);
    }
}