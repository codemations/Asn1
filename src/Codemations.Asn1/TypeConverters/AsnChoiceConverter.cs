using System;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;

namespace Codemations.Asn1.TypeConverters
{
    internal class AsnChoiceConverter : AsnRootConverter
    {
        public override bool IsAccepted(Type type)
        {
            return type.GetCustomAttribute<AsnChoiceAttribute>() is not null;
        }

        public override object Read(AsnReader reader, Type type)
        {
            var item = Activator.CreateInstance(type)!;
            var tag = reader.PeekTag();
            var propertyInfos = GetPropertyInfos(item, true)
                .Where(x => x.GetCustomAttribute<AsnElementAttribute>()!.Tag == tag).ToArray();

            switch (propertyInfos.Length)
            {
                case 0:
                    throw new AsnConversionException("No choice element with given tag.", tag);

                case 1:
                    var propertyInfo = propertyInfos.Single();
                    var asnElementAttribute = propertyInfo.GetCustomAttribute<AsnElementAttribute>()!;
                    var value = asnElementAttribute.Converter.Read(reader, asnElementAttribute.Tag, propertyInfo.PropertyType);
                    propertyInfo.SetValue(item, value);
                    return item;

                default:
                    throw new AsnConversionException("Multiple choice elements with given tag.", tag);
            }
        }

        public override void Write(AsnWriter writer, object item)
        {
            var propertyInfos = GetPropertyInfos(item).ToArray();

            switch (propertyInfos.Length)
            {
                case 0:
                    throw new AsnConversionException("No choice element to serialize.");

                case 1:
                    var propertyInfo = propertyInfos.Single();
                    var asnElementAttribute = propertyInfo.GetCustomAttribute<AsnElementAttribute>()!;
                    var value = propertyInfo.GetValue(item)!;
                    asnElementAttribute.Converter.Write(writer, asnElementAttribute.Tag, value);
                    break;

                default:
                    throw new AsnConversionException("Multiple non-null choice elements.");
            }
        }
    }
}