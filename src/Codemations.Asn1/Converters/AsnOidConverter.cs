using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters
{
    internal class AsnOidConverter : IAsnConverter
    {
        public bool CanConvert(Type type)
        {
            return type == typeof(AsnOid);
        }

        public object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
        {
            var oidStr = reader.ReadObjectIdentifier(tag);
            return new AsnOid(oidStr, false);
        }

        public void Write(AsnWriter writer, Asn1Tag? tag, object value, AsnSerializer serializer)
        {
            writer.WriteObjectIdentifier((string)(AsnOid)value, tag);
        }
    }
}