using System;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;

namespace Codemations.Asn1.Converters
{
    internal class AsnSequenceConverter : IAsnConverter
    {
        public bool CanConvert(Type type)
        {
            return type.IsClass;
        }

        public object Read(AsnReader reader, Asn1Tag? tag, Type type, IAsnConverterResolver converterResolver)
        {
            var sequenceReader = reader.ReadSequence(tag);

            var item = Activator.CreateInstance(type)!;

            foreach (var propertyInfo in AsnHelper.GetPropertyInfos(type))
            {
                var asnElementAttribute = propertyInfo.GetCustomAttribute<AsnElementAttribute>()!;

                try
                {
                    var converter = converterResolver.Resolve(propertyInfo);
                    var value = converter.Read(sequenceReader, asnElementAttribute.Tag, propertyInfo.PropertyType, converterResolver);
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

        public void Write(AsnWriter writer, Asn1Tag? tag, object value, IAsnConverterResolver converterResolver)
        {
            writer.PushSequence(tag);
            foreach (var propertyInfo in AsnHelper.GetPropertyInfos(value.GetType())
                         .Where(propertyInfo => propertyInfo.GetValue(value) is not null))
            {
                var asnElementAttribute = propertyInfo.GetCustomAttribute<AsnElementAttribute>()!;
                var propertyValue = propertyInfo.GetValue(value)!;
                var converter = converterResolver.Resolve(propertyInfo);
                converter.Write(writer, asnElementAttribute.Tag, propertyValue, converterResolver);
            }
            writer.PopSequence(tag);
        }
    }
}