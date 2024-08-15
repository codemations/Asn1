using System;
using System.Formats.Asn1;
using System.Linq;

namespace Codemations.Asn1.Converters
{
    internal class AsnOidConverter : IAsnConverter
    {
        private static readonly Type[] AcceptedTypes = 
        {
            typeof(AsnOid), typeof(AsnOid?)
        };

        public bool CanConvert(Type type)
        {
            return AcceptedTypes.Contains(type);
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