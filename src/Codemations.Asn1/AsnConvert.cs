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
                    yield return new AsnConstructedElement(tag, elements);
                }
                else
                {
                    yield return new AsnPrimitiveElement(tag) { Value = value.ToArray() };
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
                switch (element)
                {
                    case AsnConstructedElement constructed:
                        writer.PushSequence(constructed.Tag);
                        Serialize(writer, constructed.Elements);
                        writer.PopSequence(constructed.Tag);
                        break;

                    case AsnPrimitiveElement {Value: null}:
                        writer.WriteNull(element.Tag);
                        break;

                    case AsnPrimitiveElement primitive:
                        writer.WriteOctetString(primitive.Value.ToArray(), primitive.Tag);
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
            foreach (var property in item.GetType().GetProperties())
            {
                if (property.GetCustomAttribute(typeof(AsnElementAttribute)) is AsnElementAttribute element &&
                    property.GetValue(item) is { } value)
                {
                    var tag = element.Tag;
                    if (tag.IsConstructed)
                    {
                        writer.PushSequence(tag);
                        Serialize(writer, value);
                        writer.PopSequence(tag);
                    }
                    else
                    {
                        element.Converter.Write(writer, tag, value);
                    }
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
            while (reader.HasData)
            {
                foreach (var property in item.GetType().GetProperties())
                {
                    var asn1Tag = reader.PeekTag();
                    if (property.GetCustomAttribute(typeof(AsnElementAttribute)) is AsnElementAttribute element &&
                        asn1Tag.ToByte() == element.Tag.ToByte())
                    {
                        if (asn1Tag.IsConstructed)
                        {
                            var value = Activator.CreateInstance(property.PropertyType)!;
                            property.SetValue(item, value);
                            Deserialize(reader.ReadSequence(asn1Tag), value);
                        }
                        else
                        {
                            var value = element.Converter.Read(reader, asn1Tag, property.PropertyType);
                            property.SetValue(item, value);
                        }
                    }
                }
            }
        }
    }
}
