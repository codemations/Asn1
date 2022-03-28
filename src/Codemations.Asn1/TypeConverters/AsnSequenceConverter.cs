using System;
using System.Formats.Asn1;
using System.Reflection;

namespace Codemations.Asn1.TypeConverters
{
    internal class AsnSequenceConverter : AsnRootConverter
    {
        public override bool IsAccepted(Type type)
        {
            return type.IsClass;
        }

        public override object Read(AsnReader reader, Type type)
        {
            var item = Activator.CreateInstance(type)!;

            foreach (var propertyInfo in GetPropertyInfos(item, true))
            {
                var asnElementAttribute = propertyInfo.GetCustomAttribute<AsnElementAttribute>()!;

                try
                {
                    var value = asnElementAttribute.Converter.Read(reader, asnElementAttribute.Tag, propertyInfo.PropertyType);
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
            foreach (var propertyInfo in GetPropertyInfos(item))
            {
                var asnElementAttribute = propertyInfo.GetCustomAttribute<AsnElementAttribute>()!;
                var value = propertyInfo.GetValue(item)!;
                var tag = asnElementAttribute.Tag;

                asnElementAttribute.Converter.Write(writer, tag, value);
            }
        }
    }
}