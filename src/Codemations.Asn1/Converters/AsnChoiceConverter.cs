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
            var propertyInfos = AsnHelper.GetPropertyInfos(type)
                .Where(x => x.GetCustomAttribute<AsnElementAttribute>()!.Tag == innerTag).ToArray();

            switch (propertyInfos.Length)
            {
                case 0:
                    throw new AsnConversionException("No choice element with given tag.", innerTag);

                case 1:
                    var item = Activator.CreateInstance(type)!;
                    var propertyInfo = propertyInfos.Single();
                    var asnElementAttribute = propertyInfo.GetCustomAttribute<AsnElementAttribute>()!;
                    var converter = converterResolver.Resolve(propertyInfo);
                    var value = converter.Read(choiceReader, asnElementAttribute.Tag, propertyInfo.PropertyType, converterResolver);
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

            var propertyInfos = AsnHelper.GetPropertyInfos(value.GetType())
                .Where(propertyInfo => propertyInfo.GetValue(value) is not null).ToArray();

            switch (propertyInfos.Length)
            {
                case 0:
                    throw new AsnConversionException("No choice element to serialize.");

                case 1:
                    var propertyInfo = propertyInfos.Single();
                    var asnElementAttribute = propertyInfo.GetCustomAttribute<AsnElementAttribute>()!;
                    var propertyValue = propertyInfo.GetValue(value)!;
                    var converter = converterResolver.Resolve(propertyInfo);
                    converter.Write(writer, asnElementAttribute.Tag, propertyValue, converterResolver);
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