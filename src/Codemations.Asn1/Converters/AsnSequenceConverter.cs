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

            var item = Activator.CreateInstance(type) ?? throw new AsnConversionException("Failed to create object.");

            foreach (var propertyInfo in AsnHelper.GetAsnProperties(type))
            {
                try
                {
                    var converter = converterResolver.Resolve(propertyInfo.Type);
                    var value = converter.Read(sequenceReader, propertyInfo.Tag, propertyInfo.Type, converterResolver);
                    propertyInfo.SetValue(item, value);
                }
                catch (AsnContentException e)
                {
                    if (!propertyInfo.IsOptional)
                    {
                        throw new AsnConversionException("Value for required element is missing.", propertyInfo.Tag, e);
                    }
                }
            }

            return item;
        }

        public void Write(AsnWriter writer, Asn1Tag? tag, object value, IAsnConverterResolver converterResolver)
        {
            writer.PushSequence(tag);
            foreach (var propertyInfo in AsnHelper.GetAsnProperties(value.GetType()))
            {
                if(propertyInfo.GetValue(value) is not object propertyValue)
                {
                    if (propertyInfo.IsOptional)
                    {
                        continue;
                    }
                    throw new AsnConversionException("Value for required element is missing.");
                }
                writer.WriteProperty(propertyInfo, propertyValue, converterResolver);
            }
            writer.PopSequence(tag);
        }
    }
}