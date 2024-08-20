using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using Codemations.Asn1.Extensions;

namespace Codemations.Asn1
{
    /// <summary>
    ///   Provides methods for converting between .NET types and ASN.1 encoded values.
    /// </summary>
    public static class AsnConvert
    {
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
                        writer.WriteOctetString(element.Value.Value.Span, element.Tag);
                        break;
                }
            }
        }

        public static byte[] Serialize(object element, AsnEncodingRules ruleSet)
        {
            var serializer = new AsnSerializer(ruleSet);
            return serializer.Serialize(element);
        }

        public static int Serialize(object element, AsnEncodingRules ruleSet, Span<byte> destination)
        {
            var serializer = new AsnSerializer(ruleSet);
            return serializer.Serialize(element, destination);
        }

        public static byte[] SerializeBer(object element)
        {
            return Serialize(element, AsnEncodingRules.BER);
        }

        public static int SerializeBer(object element, Span<byte> destination)
        {
            return Serialize(element, AsnEncodingRules.BER, destination);
        }

        public static byte[] SerializeCer(object element)
        {
            return Serialize(element, AsnEncodingRules.CER);
        }

        public static int SerializeCer(object element, Span<byte> destination)
        {
            return Serialize(element, AsnEncodingRules.CER, destination);
        }

        public static byte[] SerializeDer(object element)
        {
            return Serialize(element, AsnEncodingRules.DER);
        }

        public static int SerializeDer(object element, Span<byte> destination)
        {
            return Serialize(element, AsnEncodingRules.DER, destination);
        }

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
                var value = reader.ReadContentBytes(out var tag);

                if (tag.IsConstructed)
                {
                    var elements = Deserialize(value, ruleSet, options).ToList();
                    yield return new AsnElement(tag, elements);
                }
                else
                {
                    yield return new AsnElement(tag) { Value = value };
                }
            }
        }

        public static object Deserialize(ReadOnlyMemory<byte> data, Type type, AsnEncodingRules ruleSet, AsnReaderOptions options = default)
        {
            var serializer = new AsnSerializer(ruleSet, options);
            return serializer.Deserialize(data, type);
        }

        public static T Deserialize<T>(ReadOnlyMemory<byte> data, AsnEncodingRules ruleSet, AsnReaderOptions options = default)
        {
            var deserialized = Deserialize(data, typeof(T), ruleSet, options);
            return (T)deserialized;
        }

        public static object DeserializeBer(ReadOnlyMemory<byte> data, Type type, AsnReaderOptions options = default)
        {
            return Deserialize(data, type, AsnEncodingRules.BER, options);
        }

        public static T DeserializeBer<T>(ReadOnlyMemory<byte> data, AsnReaderOptions options = default)
        {
            return Deserialize<T>(data, AsnEncodingRules.BER, options);
        }

        public static object DeserializeCer(ReadOnlyMemory<byte> data, Type type, AsnReaderOptions options = default)
        {
            return Deserialize(data, type, AsnEncodingRules.CER, options);
        }

        public static T DeserializeCer<T>(ReadOnlyMemory<byte> data, AsnReaderOptions options = default)
        {
            return Deserialize<T>(data, AsnEncodingRules.CER, options);
        }

        public static object DeserializeDer(ReadOnlyMemory<byte> data, Type type, AsnReaderOptions options = default)
        {
            return Deserialize(data, type, AsnEncodingRules.DER, options);
        }

        public static T DeserializeDer<T>(ReadOnlyMemory<byte> data, AsnReaderOptions options = default)
        {
            return Deserialize<T>(data, AsnEncodingRules.DER, options);
        }
    }
}
