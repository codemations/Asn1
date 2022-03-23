using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;

namespace Codemations.Asn1
{   
    /// <summary>
    ///   Provides methods for converting between .NET types and ASN.1 encoded values.
    /// </summary>
    public static class AsnConvert
    {
        /// <summary>
        ///   Deserializes <paramref name="data"/> with a given <paramref name="ruleSet"/>.
        /// </summary>
        /// <param name="data">The data to read.</param>
        /// <param name="ruleSet">The encoding constraints for the reader.</param>
        /// <param name="options">Additional options for the reader.</param>
        /// <returns>An iterator with AsnElement objects.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="ruleSet"/> is not defined.
        /// </exception>
        public static IEnumerable<AsnElement> Deserialize(ReadOnlyMemory<byte> data, AsnEncodingRules ruleSet, AsnReaderOptions options = default)
        {
            var reader = new AsnReader(data, ruleSet, options);
            while (reader.HasData)
            {
                var tag = reader.PeekTag();
                var value = reader.PeekContentBytes();

                if (tag.IsConstructed)
                {
                    var elements = Deserialize(value, ruleSet, options).ToList();
                    yield return new AsnElement(tag, elements);
                }
                else
                {
                    yield return new AsnElement(tag) { Value = value.ToArray() };
                }

                reader.ReadEncodedValue();
            }
        }

        /// <summary>
        ///   Serializes <paramref name="items"/> with a given <paramref name="ruleSet"/>.
        /// </summary>
        /// <param name="items">Elements to serialize.</param>
        /// <param name="ruleSet">The encoding constraints for the reader.</param>
        /// <returns>Encoded data.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="ruleSet"/> is not defined.
        /// </exception>
        public static byte[] Serialize(IEnumerable<AsnElement> items, AsnEncodingRules ruleSet)
        {
            var writer = new AsnWriter(ruleSet);
            Serialize(writer, items);
            return writer.Encode();
        }

        private static void Serialize(AsnWriter writer, IEnumerable<AsnElement> items)
        {
            foreach (var element in items)
            {
                switch (element.Tag.IsConstructed)
                {
                    case true:
                        writer.PushSequence(element.Tag);
                        Serialize(writer, element.Elements);
                        writer.PopSequence(element.Tag);
                        break;

                    case false when element.Value is null:
                        writer.WriteNull(element.Tag);
                        break;

                    case false:
                        writer.WriteOctetString(element.Value.ToArray(), element.Tag);
                        break;
                }
            }
        }

        public static byte[] Serialize(object element, AsnEncodingRules ruleSet)
        {
            var writer = new AsnWriter(ruleSet);
            Serialize(writer, element);
            return writer.Encode();
        }

        private static void Serialize(AsnWriter writer, object item)
        {
            foreach (var property in item.GetPropertyInfos<AsnElementAttribute>(false))
            {
                var asnElementAttribute = property.GetCustomAttribute<AsnElementAttribute>()!;
                var value = property.GetValue(item)!;
                var tag = asnElementAttribute.Tag;

                if (tag.IsConstructed)
                {
                    writer.PushSequence(tag);
                    Serialize(writer, value);
                    writer.PopSequence(tag);
                }
                else
                {
                    asnElementAttribute.Converter.Write(writer, tag, value);
                }
            }
        }

        public static T Deserialize<T>(ReadOnlyMemory<byte> data, AsnEncodingRules ruleSet, AsnReaderOptions options = default) where T : class, new()
        {
            var reader = new AsnReader(data, ruleSet, options);
            var deserialized = new T();
            Deserialize(reader, deserialized);
            return deserialized;
        }

        private static void Deserialize(AsnReader reader, object item)
        {
            if (item.GetType().GetCustomAttribute<AsnChoiceAttribute>() is not null)
            {
                DeserializeChoice(reader, item);
            }
            else
            {
                DeserializeSequence(reader, item);
            }
        }

        private static void DeserializeChoice(AsnReader reader, object item)
        {
            var tag = reader.PeekTag();
            var property = item.GetPropertyInfos<AsnElementAttribute>(true)
                .Where(x => x.GetCustomAttribute<AsnElementAttribute>()!.Tag == tag).ToArray();

            switch (property.Length)
            {
                case 0:
                    throw new AsnConversionException("");

                case 1:
                    var asnElementAttribute = property.Single().GetCustomAttribute<AsnElementAttribute>()!;
                    DeserializeElement(reader, item, property.Single(), asnElementAttribute.Converter);
                    break;

                default:
                    throw new AsnConversionException("");
            }

            if (reader.HasData)
            {
                throw new AsnConversionException("");
            }
        }

        private static void DeserializeSequence(AsnReader reader, object item)
        {
            foreach (var property in item.GetPropertyInfos<AsnElementAttribute>(true))
            {
                var asnElementAttribute = property.GetCustomAttribute<AsnElementAttribute>()!;

                if (!TryDeserializeElement(reader, item, property) && !asnElementAttribute.Optional)
                {
                    throw new AsnConversionException("Value for required element is missing.", asnElementAttribute.Tag);
                }
            }
        }

        private static bool TryDeserializeElement(AsnReader reader, object item, PropertyInfo propertyInfo)
        {
            if (!reader.HasData)
            {
                return false;
            }

            var asnElementAttribute = propertyInfo.GetCustomAttribute<AsnElementAttribute>()!;
            if (reader.PeekTag().ToByte() != asnElementAttribute.Tag.ToByte())
            {
                return false;
            }

            DeserializeElement(reader, item, propertyInfo, asnElementAttribute.Converter);

            return true;
        }

        private static void DeserializeElement(AsnReader reader, object item, PropertyInfo propertyInfo, IAsnConverter converter)
        {
            var tag = reader.PeekTag();

            if (tag.IsConstructed)
            {
                var value = Activator.CreateInstance(propertyInfo.PropertyType)!;
                propertyInfo.SetValue(item, value);
                Deserialize(reader.ReadSequence(tag), value);
            }
            else
            {
                var value = converter.Read(reader, tag, propertyInfo.PropertyType);
                propertyInfo.SetValue(item, value);
            }
        }
    }
}
