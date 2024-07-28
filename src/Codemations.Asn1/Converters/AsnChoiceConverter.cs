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

        private void CheckRead(int propertiesLength, Asn1Tag expectedTag)
        {
            if(propertiesLength == 0)
            {
                throw new AsnConversionException("No choice element with given tag.", expectedTag);
            }
            if(propertiesLength > 1) 
            {
                throw new AsnConversionException("Multiple choice elements with given tag.", expectedTag);
            }
        }

        public object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
        {
            var choiceReader = tag is null ? reader : reader.ReadSequence(tag);

            var innerTag = choiceReader.PeekTag();
            var propertyInfos = AsnHelper.GetAsnProperties(type)
                .Where(propertyInfo => propertyInfo.Tag == innerTag).ToArray();

            CheckRead(propertyInfos.Length, innerTag);

            var item = type.CreateInstance();
            var propertyInfo = propertyInfos.Single();
            var value = serializer.Deserialize(choiceReader, propertyInfo.Tag, propertyInfo.Type);
            propertyInfo.SetValue(item, value);
            return item;
        }

        public void Write(AsnWriter writer, Asn1Tag? tag, object value, AsnSerializer serializer)
        {
            if (tag is not null)
            {
                writer.PushSequence(tag);
            }

            var propertyInfos = AsnHelper.GetAsnProperties(value.GetType())
                .Where(x => x.GetValue(value) is not null).ToArray();

            switch (propertyInfos.Length)
            {
                case 0:
                    throw new AsnConversionException("No choice element to serialize.");

                case 1:
                    var propertyInfo = propertyInfos.Single();
                    var propertyValue = propertyInfo.GetValue(value)!;
                    serializer.Serialize(writer, propertyInfo.Tag, propertyValue);
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