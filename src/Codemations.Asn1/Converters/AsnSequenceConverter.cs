using System;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;

namespace Codemations.Asn1.Converters
{
    internal class AsnSequenceConverter : AsnConstructedConverter
    {
        public AsnSequenceConverter(AsnConverterFactory converterFactory) : base(converterFactory)
        {
        }

        public override bool IsAccepted(Type type)
        {
            return type.IsClass;
        }

        public override object Read(AsnReader reader, Asn1Tag? tag, Type type)
        {
            var sequenceReader = reader.ReadSequence(tag);

            var item = Activator.CreateInstance(type)!;

            foreach (var propertyInfo in GetPropertyInfos(type))
            {
                var asnElementAttribute = propertyInfo.GetCustomAttribute<AsnElementAttribute>()!;

                try
                {
                    var converter = GetConverter(asnElementAttribute, propertyInfo.PropertyType);
                    var value = converter.Read(sequenceReader, asnElementAttribute.Tag, propertyInfo.PropertyType);
                    propertyInfo.SetValue(item, value);
                }
                catch (AsnContentException e)
                {
                    if (!asnElementAttribute.Optional)
                    {
                        throw new AsnConversionException("Value for required element is missing.", asnElementAttribute.Tag, e);
                    }
                }
            }

            return item;
        }

        public override void Write(AsnWriter writer, Asn1Tag? tag, object item)
        {
            writer.PushSequence(tag);
            foreach (var propertyInfo in GetPropertyInfos(item.GetType())
                         .Where(propertyInfo => propertyInfo.GetValue(item) is not null))
            {
                var asnElementAttribute = propertyInfo.GetCustomAttribute<AsnElementAttribute>()!;
                var value = propertyInfo.GetValue(item)!;
                var converter = GetConverter(asnElementAttribute, propertyInfo.PropertyType);
                converter.Write(writer, asnElementAttribute.Tag, value);
            }
            writer.PopSequence(tag);
        }
    }
}