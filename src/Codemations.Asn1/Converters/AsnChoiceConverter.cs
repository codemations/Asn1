using System;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;

namespace Codemations.Asn1.Converters
{
    internal class AsnChoiceConverter : IAsnConverter
    {
        public bool CanConvert(Type type)
        {
            return type.GetCustomAttribute<AsnChoiceAttribute>() is not null;
        }

        public object Read(AsnReader reader, Asn1Tag? tag, Type type, IAsnConverterResolver converterResolver)
        {
            var choiceReader = tag is null ? reader : reader.ReadSequence(tag);

            var innerTag = choiceReader.PeekTag();
            var propertyInfos = AsnHelper.GetAsnProperties(type)
                .Where(propertyInfo => propertyInfo.Tag == innerTag).ToArray();

            switch (propertyInfos.Length)
            {
                case 0:
                    throw new AsnConversionException("No choice element with given tag.", innerTag);

                case 1:
                    var item = Activator.CreateInstance(type)!;
                    var propertyInfo = propertyInfos.Single();
                    var converter = converterResolver.Resolve(propertyInfo.Type);
                    var value = converter.Read(choiceReader, propertyInfo.Tag, propertyInfo.Type, converterResolver);
                    propertyInfo.SetValue(item, value);
                    return item;

                default:
                    throw new AsnConversionException("Multiple choice elements with given tag.", innerTag);
            }
        }

        public void Write(AsnWriter writer, Asn1Tag? tag, object value, IAsnConverterResolver converterResolver)
        {
            if (tag is not null)
            {
                writer.PushSequence(tag);
            }

            var properties = AsnHelper.GetAsnProperties(value.GetType())
                .Where(x => x.GetValue(value) is not null).ToArray();

            switch (properties.Length)
            {
                case 0:
                    throw new AsnConversionException("No choice element to serialize.");

                case 1:
                    var property = properties.Single();
                    var converter = property.CustomConverter ?? converterResolver.Resolve(property.Type);
                    converter.Write(writer, property.Tag, property.GetValue(value)!, converterResolver);
                    break;

                default:
                    throw new AsnConversionException("Multiple non-null choice elements.");
            }

            if (tag is not null)
            {
                writer.PopSequence(tag);
            }
        }
    }
}