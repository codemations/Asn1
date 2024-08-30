using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using Codemations.Asn1.Extensions;

namespace Codemations.Asn1
{
    public class AsnSerializer
    {
        private readonly AsnConverterResolver _converterResolver;

        public AsnSerializer()
        {
            _converterResolver = new AsnConverterResolver();
        }

        /// <summary>
        /// Serializes a collection of <see cref="AsnElement"/> using the specified <paramref name="ruleSet"/>.
        /// </summary>
        /// <param name="items">The elements to serialize.</param>
        /// <param name="ruleSet">The encoding constraints for the writer.</param>
        /// <returns>The encoded data as a byte array.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="ruleSet"/> is not defined.
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
                if (element.Tag.IsConstructed)
                {
                    writer.PushSequence(element.Tag);
                    Serialize(writer, element.Elements);
                    writer.PopSequence(element.Tag);
                }
                else if (element.Value is null)
                {
                    writer.WriteNull(element.Tag);
                }
                else
                {
                    writer.WriteOctetString(element.Value.Value.Span, element.Tag);
                }
            }
        }

        /// <summary>
        /// Serializes an object using the specified <paramref name="ruleSet"/>.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="ruleSet">The encoding rules to apply.</param>
        /// <returns>The encoded data as a byte array.</returns>
        public static byte[] Serialize(object value, AsnEncodingRules ruleSet)
        {
            var writer = SerializeInternal(value, ruleSet);
            return writer.Encode();
        }

        /// <summary>
        /// Serializes an object to a given <paramref name="destination"/> using the specified <paramref name="ruleSet"/>.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="destination">The buffer to write the encoded data to.</param>
        /// <param name="ruleSet">The encoding rules to apply.</param>
        /// <returns>The number of bytes written.</returns>
        public static int Serialize(object value, Span<byte> destination, AsnEncodingRules ruleSet)
        {
            var writer = SerializeInternal(value, ruleSet);
            return writer.Encode(destination);
        }

        /// <summary>
        /// Serializes an object to the specified <see cref="AsnWriter"/>.
        /// </summary>
        /// <param name="writer">The writer to serialize the object into.</param>
        /// <param name="value">The object to serialize.</param>
        public void Serialize(AsnWriter writer, object value)
        {
            var converter = _converterResolver.Resolve(value.GetType(), out _);
            SerializeCore(writer, tag: null, value, converter);
        }

        /// <summary>
        /// Serializes an object to the specified <see cref="AsnWriter"/> with additional property information.
        /// </summary>
        /// <param name="writer">The writer to serialize the object into.</param>
        /// <param name="asnPropertyInfo">The ASN.1 property information.</param>
        /// <param name="value">The object to serialize.</param>
        public void Serialize(AsnWriter writer, AsnPropertyInfo asnPropertyInfo, object value)
        {
            var converter = _converterResolver.Resolve(asnPropertyInfo, out _);
            SerializeCore(writer, asnPropertyInfo.Tag, value, converter);
        }

        private static AsnWriter SerializeInternal(object value, AsnEncodingRules ruleSet)
        {
            var writer = new AsnWriter(ruleSet);
            new AsnSerializer().Serialize(writer, value);
            return writer;
        }

        private void SerializeCore(AsnWriter writer, Asn1Tag? tag, object value, AsnConverter converter)
        {            
            converter.Write(writer, tag, value, this);
        }

        /// <summary>
        /// Serializes an object using BER encoding rules.
        /// </summary>
        /// <param name="element">The object to serialize.</param>
        /// <returns>The encoded data as a byte array.</returns>
        public static byte[] SerializeBer(object element)
        {
            return Serialize(element, AsnEncodingRules.BER);
        }

        /// <summary>
        /// Serializes an object to a given <paramref name="destination"/> using BER encoding rules.
        /// </summary>
        /// <param name="element">The object to serialize.</param>
        /// <param name="destination">The buffer to write the encoded data to.</param>
        /// <returns>The number of bytes written.</returns>
        public static int SerializeBer(object element, Span<byte> destination)
        {
            return Serialize(element, destination, AsnEncodingRules.BER);
        }

        /// <summary>
        /// Serializes an object using CER encoding rules.
        /// </summary>
        /// <param name="element">The object to serialize.</param>
        /// <returns>The encoded data as a byte array.</returns>
        public static byte[] SerializeCer(object element)
        {
            return Serialize(element, AsnEncodingRules.CER);
        }

        /// <summary>
        /// Serializes an object to a given <paramref name="destination"/> using CER encoding rules.
        /// </summary>
        /// <param name="element">The object to serialize.</param>
        /// <param name="destination">The buffer to write the encoded data to.</param>
        /// <returns>The number of bytes written.</returns>
        public static int SerializeCer(object element, Span<byte> destination)
        {
            return Serialize(element, destination, AsnEncodingRules.CER);
        }

        /// <summary>
        /// Serializes an object using DER encoding rules.
        /// </summary>
        /// <param name="element">The object to serialize.</param>
        /// <returns>The encoded data as a byte array.</returns>
        public static byte[] SerializeDer(object element)
        {
            return Serialize(element, AsnEncodingRules.DER);
        }

        /// <summary>
        /// Serializes an object to a given <paramref name="destination"/> using DER encoding rules.
        /// </summary>
        /// <param name="element">The object to serialize.</param>
        /// <param name="destination">The buffer to write the encoded data to.</param>
        /// <returns>The number of bytes written.</returns>
        public static int SerializeDer(object element, Span<byte> destination)
        {
            return Serialize(element, destination, AsnEncodingRules.DER);
        }

        /// <summary>
        /// Deserializes the specified <paramref name="data"/> using the given <paramref name="ruleSet"/>.
        /// </summary>
        /// <param name="data">The encoded data to deserialize.</param>
        /// <param name="ruleSet">The encoding rules for deserialization.</param>
        /// <param name="options">Additional options for the reader.</param>
        /// <returns>An enumerable of <see cref="AsnElement"/> objects.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="ruleSet"/> is not defined.
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

        /// <summary>
        /// Deserializes the specified <paramref name="data"/> into an object of the given <paramref name="type"/>.
        /// </summary>
        /// <param name="data">The encoded data to deserialize.</param>
        /// <param name="type">The type to deserialize into.</param>
        /// <param name="ruleSet">The encoding rules for deserialization.</param>
        /// <param name="options">Additional options for the reader.</param>
        /// <returns>The deserialized object.</returns>
        public static object Deserialize(ReadOnlyMemory<byte> data, Type type, AsnEncodingRules ruleSet, AsnReaderOptions options = default)
        {
            var reader = new AsnReader(data, ruleSet, options);
            var value = new AsnSerializer().Deserialize(reader, type);

            reader.ThrowIfNotEmpty();

            return value;
        }

        /// <summary>
        /// Deserializes data from the specified <see cref="AsnReader"/> into an object of the given <paramref name="type"/>.
        /// </summary>
        /// <param name="reader">The reader to deserialize from.</param>
        /// <param name="type">The type to deserialize into.</param>
        /// <returns>The deserialized object.</returns>
        public object Deserialize(AsnReader reader, Type type)
        {
            var converter = _converterResolver.Resolve(type, out var resolvedType);
            return Deserialize(reader, tag: null, resolvedType, converter);
        }

        /// <summary>
        /// Deserializes data from the specified <see cref="AsnReader"/> into an object using additional property information.
        /// </summary>
        /// <param name="reader">The reader to deserialize from.</param>
        /// <param name="asnPropertyInfo">The ASN.1 property information.</param>
        /// <returns>The deserialized object.</returns>
        public object Deserialize(AsnReader reader, AsnPropertyInfo asnPropertyInfo)
        {
            var converter = _converterResolver.Resolve(asnPropertyInfo, out var resolvedType);
            return Deserialize(reader, asnPropertyInfo.Tag, resolvedType, converter);
        }

        /// <summary>
        /// Deserializes data from the specified <see cref="AsnReader"/> into an object of the given <paramref name="type"/> using the provided <paramref name="tag"/> and optional custom converter.
        /// </summary>
        /// <param name="reader">The reader to deserialize from.</param>
        /// <param name="tag">The expected tag for the ASN.1 data.</param>
        /// <param name="type">The type to deserialize into.</param>
        /// <param name="converter">An optional custom converter to use during deserialization.</param>
        /// <returns>The deserialized object.</returns>
        public object Deserialize(AsnReader reader, Asn1Tag? tag, Type type, AsnConverter converter)
        {
            return converter.Read(reader, tag, type, this);
        }

        /// <summary>
        /// Deserializes the specified <paramref name="data"/> into an object of type <typeparamref name="T"/> using the given <paramref name="ruleSet"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize into.</typeparam>
        /// <param name="data">The encoded data to deserialize.</param>
        /// <param name="ruleSet">The encoding rules for deserialization.</param>
        /// <param name="options">Additional options for the reader.</param>
        /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
        public static T Deserialize<T>(ReadOnlyMemory<byte> data, AsnEncodingRules ruleSet, AsnReaderOptions options = default)
        {
            var deserialized = Deserialize(data, typeof(T), ruleSet, options);
            return (T)deserialized;
        }

        /// <summary>
        /// Deserializes data using BER encoding rules into an object of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="data">The encoded data to deserialize.</param>
        /// <param name="type">The type to deserialize into.</param>
        /// <param name="options">Additional options for the reader.</param>
        /// <returns>The deserialized object.</returns>
        public static object DeserializeBer(ReadOnlyMemory<byte> data, Type type, AsnReaderOptions options = default)
        {
            return Deserialize(data, type, AsnEncodingRules.BER, options);
        }

        /// <summary>
        /// Deserializes data using BER encoding rules into an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize into.</typeparam>
        /// <param name="data">The encoded data to deserialize.</param>
        /// <param name="options">Additional options for the reader.</param>
        /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
        public static T DeserializeBer<T>(ReadOnlyMemory<byte> data, AsnReaderOptions options = default)
        {
            return Deserialize<T>(data, AsnEncodingRules.BER, options);
        }

        /// <summary>
        /// Deserializes data using CER encoding rules into an object of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="data">The encoded data to deserialize.</param>
        /// <param name="type">The type to deserialize into.</param>
        /// <param name="options">Additional options for the reader.</param>
        /// <returns>The deserialized object.</returns>
        public static object DeserializeCer(ReadOnlyMemory<byte> data, Type type, AsnReaderOptions options = default)
        {
            return Deserialize(data, type, AsnEncodingRules.CER, options);
        }

        /// <summary>
        /// Deserializes data using CER encoding rules into an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize into.</typeparam>
        /// <param name="data">The encoded data to deserialize.</param>
        /// <param name="options">Additional options for the reader.</param>
        /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
        public static T DeserializeCer<T>(ReadOnlyMemory<byte> data, AsnReaderOptions options = default)
        {
            return Deserialize<T>(data, AsnEncodingRules.CER, options);
        }

        /// <summary>
        /// Deserializes data using DER encoding rules into an object of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="data">The encoded data to deserialize.</param>
        /// <param name="type">The type to deserialize into.</param>
        /// <param name="options">Additional options for the reader.</param>
        /// <returns>The deserialized object.</returns>
        public static object DeserializeDer(ReadOnlyMemory<byte> data, Type type, AsnReaderOptions options = default)
        {
            return Deserialize(data, type, AsnEncodingRules.DER, options);
        }

        /// <summary>
        /// Deserializes data using DER encoding rules into an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize into.</typeparam>
        /// <param name="data">The encoded data to deserialize.</param>
        /// <param name="options">Additional options for the reader.</param>
        /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
        public static T DeserializeDer<T>(ReadOnlyMemory<byte> data, AsnReaderOptions options = default)
        {
            return Deserialize<T>(data, AsnEncodingRules.DER, options);
        }
    }
}
