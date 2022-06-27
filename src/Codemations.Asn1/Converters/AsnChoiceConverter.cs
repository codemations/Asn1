using System;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;

namespace Codemations.Asn1.Converters
{
    internal class AsnChoiceConverter : AsnConstructedConverter
    {
        public AsnChoiceConverter(AsnConverterFactory converterFactory) : base(converterFactory)
        {
        }

        public override bool IsAccepted(Type type)
        {
            return type.GetCustomAttribute<AsnChoiceAttribute>() is not null;
        }

        public override object Read(AsnReader reader, Asn1Tag? tag, Type type)
        {
            var choiceReader = tag is null ? reader : reader.ReadSequence(tag);

            var innerTag = choiceReader.PeekTag();
            var propertyInfos = GetPropertyInfos(type)
                .Where(x => x.GetCustomAttribute<AsnElementAttribute>()!.Tag == innerTag).ToArray();

            switch (propertyInfos.Length)
            {
                case 0:
                    throw new AsnConversionException("No choice element with given tag.", innerTag);

                case 1:
                    var item = Activator.CreateInstance(type)!;
                    var propertyInfo = propertyInfos.Single();
                    var asnElementAttribute = propertyInfo.GetCustomAttribute<AsnElementAttribute>()!;
                    var converter = GetConverter(asnElementAttribute, propertyInfo.PropertyType);
                    var value = converter.Read(choiceReader, asnElementAttribute.Tag, propertyInfo.PropertyType);
                    propertyInfo.SetValue(item, value);
                    return item;

                default:
                    throw new AsnConversionException("Multiple choice elements with given tag.", innerTag);
            }
        }

        public override void Write(AsnWriter writer, Asn1Tag? tag, object item)
        {
            if (tag is not null)
            {
                writer.PushSequence(tag);
            }

            var propertyInfos = GetPropertyInfos(item.GetType())
                .Where(propertyInfo => propertyInfo.GetValue(item) is not null).ToArray();

            switch (propertyInfos.Length)
            {
                case 0:
                    throw new AsnConversionException("No choice element to serialize.");

                case 1:
                    var propertyInfo = propertyInfos.Single();
                    var asnElementAttribute = propertyInfo.GetCustomAttribute<AsnElementAttribute>()!;
                    var value = propertyInfo.GetValue(item)!;
                    var converter = GetConverter(asnElementAttribute, propertyInfo.PropertyType);
                    converter.Write(writer, asnElementAttribute.Tag, value);
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