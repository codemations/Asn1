using System;
using System.Formats.Asn1;
using System.Linq;

namespace Codemations.Asn1.TypeConverters
{
    internal abstract class AsnTypeConverter : IAsnConverter
    {
        internal virtual Type[] AcceptedTypes => Array.Empty<Type>();
        internal virtual bool IsAccepted(Type type)
        {
            return this.AcceptedTypes.Contains(type);
        }
        public abstract object Read(AsnReader reader, Asn1Tag? tag, Type type);
        public abstract void Write(AsnWriter writer, Asn1Tag? tag, object value);
    }
}