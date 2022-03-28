using System;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;

namespace Codemations.Asn1.Converters
{
    internal class AsnChoiceConverter : AsnRootConverter
    {
        public AsnChoiceConverter(AsnConverterFactory converterFactory) : base(converterFactory)
        {
        }

        public override bool IsAccepted(Type type)
        {
            return type.GetCustomAttribute<AsnChoiceAttribute>() is not null;
        }

        public override object Read(AsnReader reader, Type type)
        {
            var tag = reader.PeekTag();
            var propertyInfos = GetPropertyInfos(type)
                .Where(x => x.GetCustomAttribute<AsnElementAttribute>()!.Tag == tag).ToArray();

            switch (propertyInfos.Length)
            {
                case 0:
                    throw new AsnConversionException("No choice element with given tag.", tag);

                case 1:
                    var item = Activator.CreateInstance(type)!;
                    var propertyInfo = propertyInfos.Single();
                    var asnElementAttribute = propertyInfo.GetCustomAttribute<AsnElementAttribute>()!;
                    var converter = asnElementAttribute.Converter ?? this.ConverterFactory.CreateElementConverter(propertyInfo.PropertyType);
                    var value = converter.Read(reader, asnElementAttribute.Tag, propertyInfo.PropertyType);
                    propertyInfo.SetValue(item, value);
                    return item;

                default:
                    throw new AsnConversionException("Multiple choice elements with given tag.", tag);
            }
        }

        public override void Write(AsnWriter writer, object item)
        {
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
                    var converter = asnElementAttribute.Converter ?? this.ConverterFactory.CreateElementConverter(propertyInfo.PropertyType);
                    converter.Write(writer, asnElementAttribute.Tag, value);
                    break;

                default:
                    throw new AsnConversionException("Multiple non-null choice elements.");
            }
        }
    }
}