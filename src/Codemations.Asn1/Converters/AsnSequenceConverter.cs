using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters
{
    internal class AsnSequenceConverter : IAsnConverter
    {
        public bool CanConvert(Type type)
        {
            return type.IsClass;
        }

        public object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
        {
            var sequenceReader = reader.ReadSequence(tag);

            var item = type.CreateInstance();

            foreach (var propertyInfo in AsnHelper.GetAsnProperties(type))
            {
                try
                {
                    var value = serializer.Deserialize(sequenceReader, propertyInfo.Tag, propertyInfo.Type);
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

        public void Write(AsnWriter writer, Asn1Tag? tag, object value, AsnSerializer serializer)
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
                serializer.Serialize(writer, propertyInfo.Tag, propertyValue);
            }
            writer.PopSequence(tag);
        }
    }
}