using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;

namespace Codemations.Asn1.Converters
{
    internal abstract class AsnRootConverter : AsnElementConverter
    {
        protected AsnRootConverter(AsnConverterFactory converterFactory) : base(converterFactory)
        {
        }

        public abstract object Read(AsnReader reader, Type type);

        public abstract void Write(AsnWriter writer, object item);

        public override object Read(AsnReader reader, Asn1Tag? tag, Type type)
        {
            return Read(reader.ReadSequence(tag), type);
        }

        public override void Write(AsnWriter writer, Asn1Tag? tag, object item)
        {
            writer.PushSequence(tag);
            Write(writer, item);
            writer.PopSequence(tag);
        }

        protected static IEnumerable<PropertyInfo> GetPropertyInfos(Type type)
        {
            return type.GetProperties().Where(x => x.GetCustomAttribute<AsnElementAttribute>() is not null);
        }
    }
}