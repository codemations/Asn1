using System;
using System.Formats.Asn1;
using System.Linq;

namespace Codemations.Asn1.Converters
{
    internal abstract class AsnElementConverter : IAsnConverter
    {
        protected virtual Type[] AcceptedTypes => Array.Empty<Type>();

        protected AsnConverterFactory ConverterFactory { get; set; }

        protected AsnElementConverter(AsnConverterFactory converterFactory)
        {
            this.ConverterFactory = converterFactory;
        }

        public virtual bool IsAccepted(Type type)
        {
            return this.AcceptedTypes.Contains(type);
        }

        public abstract object Read(AsnReader reader, Asn1Tag? tag, Type type);
        public abstract void Write(AsnWriter writer, Asn1Tag? tag, object value);

        protected IAsnConverter GetConverter(AsnElementAttribute asnElementAttribute, Type type)
        {
            if (asnElementAttribute.ConverterType is not null)
            {
                return (Activator.CreateInstance(asnElementAttribute.ConverterType) as IAsnConverter)!;
            }

            return this.ConverterFactory.CreateElementConverter(type);
        }
    }
}