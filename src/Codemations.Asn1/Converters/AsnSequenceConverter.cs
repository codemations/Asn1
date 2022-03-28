using System;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;

namespace Codemations.Asn1.Converters
{
    internal class AsnSequenceConverter : AsnRootConverter
    {
        public AsnSequenceConverter(AsnConverterFactory converterFactory) : base(converterFactory)
        {
        }

        public override bool IsAccepted(Type type)
        {
            return type.IsClass;
        }

        public override object Read(AsnReader reader, Type type)
        {
            var item = Activator.CreateInstance(type)!;

            foreach (var propertyInfo in GetPropertyInfos(type))
            {
                var asnElementAttribute = propertyInfo.GetCustomAttribute<AsnElementAttribute>()!;

                try
                {
                    var converter = asnElementAttribute.Converter ?? this.ConverterFactory.CreateElementConverter(propertyInfo.PropertyType);
                    var value = converter.Read(reader, asnElementAttribute.Tag, propertyInfo.PropertyType);
                    propertyInfo.SetValue(item, value);
                }
                catch (Exception e)
                {
                    if (!asnElementAttribute.Optional)
                    {
                        throw new AsnConversionException("Value for required element is missing.", asnElementAttribute.Tag, e);
                    }
                }
            }

            return item;
        }

        public override void Write(AsnWriter writer, object item)
        {
            foreach (var propertyInfo in GetPropertyInfos(item.GetType())
                         .Where(propertyInfo => propertyInfo.GetValue(item) is not null))
            {
                var asnElementAttribute = propertyInfo.GetCustomAttribute<AsnElementAttribute>()!;
                var value = propertyInfo.GetValue(item)!;
                var tag = asnElementAttribute.Tag;
                var converter = asnElementAttribute.Converter ?? this.ConverterFactory.CreateElementConverter(propertyInfo.PropertyType);
                converter.Write(writer, tag, value);
            }
        }
    }
}