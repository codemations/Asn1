using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using Codemations.Asn1.Extensions;

namespace Codemations.Asn1;

public partial class AsnSerializer
{
    private readonly AsnConverterResolver _converterResolver;

    public AsnSerializer()
    {
        _converterResolver = new AsnConverterResolver();
    }

    /// <summary>
    /// Serializes an object using the specified <paramref name="ruleSet"/> and optional <paramref name="tag"/>.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <param name="ruleSet">The encoding rules to apply.</param>
    /// <param name="tag">An optional ASN.1 tag to be applied to the serialized object.</param>
    /// <returns>The encoded data as a byte array.</returns>
    public static byte[] Serialize(object value, AsnEncodingRules ruleSet, Asn1Tag? tag = null)
    {
        var writer = SerializeInternal(value, ruleSet, tag);
        return writer.Encode();
    }

    /// <summary>
    /// Serializes an object to a given <paramref name="destination"/> using the specified <paramref name="ruleSet"/> and optional <paramref name="tag"/>.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <param name="destination">The buffer to write the encoded data to.</param>
    /// <param name="ruleSet">The encoding rules to apply.</param>
    /// <param name="tag">An optional ASN.1 tag to be applied to the serialized object.</param>
    /// <returns>The number of bytes written.</returns>
    public static int Serialize(object value, Span<byte> destination, AsnEncodingRules ruleSet, Asn1Tag? tag = null)
    {
        var writer = SerializeInternal(value, ruleSet, tag);
        return writer.Encode(destination);
    }

    /// <summary>
    /// Serializes an object to the specified <see cref="AsnWriter"/> with an optional <paramref name="tag"/>.
    /// </summary>
    /// <param name="writer">The writer to serialize the object into.</param>
    /// <param name="value">The object to serialize.</param>
    /// <param name="tag">An optional ASN.1 tag to be applied to the serialized object.</param>
    public void Serialize(AsnWriter writer, object value, Asn1Tag? tag = null)
    {
        var converter = _converterResolver.Resolve(value.GetType(), out _);
        SerializeCore(writer, value, converter, tag);
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
        SerializeCore(writer, value, converter, asnPropertyInfo.Tag);
    }

    private static AsnWriter SerializeInternal(object value, AsnEncodingRules ruleSet, Asn1Tag? tag)
    {
        var writer = new AsnWriter(ruleSet);
        new AsnSerializer().Serialize(writer, value, tag);
        return writer;
    }

    private void SerializeCore(AsnWriter writer, object value, AsnConverter converter, Asn1Tag? tag = null)
    {
        converter.Write(writer, tag, value, this);
    }

    /// <summary>
    /// Serializes an object using BER encoding rules with an optional <paramref name="tag"/>.
    /// </summary>
    /// <param name="element">The object to serialize.</param>
    /// <param name="tag">An optional ASN.1 tag to be applied to the serialized object.</param>
    /// <returns>The encoded data as a byte array.</returns>
    public static byte[] SerializeBer(object element, Asn1Tag? tag = null)
    {
        return Serialize(element, AsnEncodingRules.BER, tag);
    }

    /// <summary>
    /// Serializes an object to a given <paramref name="destination"/> using BER encoding rules with an optional <paramref name="tag"/>.
    /// </summary>
    /// <param name="element">The object to serialize.</param>
    /// <param name="destination">The buffer to write the encoded data to.</param>
    /// <param name="tag">An optional ASN.1 tag to be applied to the serialized object.</param>
    /// <returns>The number of bytes written.</returns>
    public static int SerializeBer(object element, Span<byte> destination, Asn1Tag? tag = null)
    {
        return Serialize(element, destination, AsnEncodingRules.BER, tag);
    }

    /// <summary>
    /// Serializes an object using CER encoding rules with an optional <paramref name="tag"/>.
    /// </summary>
    /// <param name="element">The object to serialize.</param>
    /// <param name="tag">An optional ASN.1 tag to be applied to the serialized object.</param>
    /// <returns>The encoded data as a byte array.</returns>
    public static byte[] SerializeCer(object element, Asn1Tag? tag = null)
    {
        return Serialize(element, AsnEncodingRules.CER, tag);
    }

    /// <summary>
    /// Serializes an object to a given <paramref name="destination"/> using CER encoding rules with an optional <paramref name="tag"/>.
    /// </summary>
    /// <param name="element">The object to serialize.</param>
    /// <param name="destination">The buffer to write the encoded data to.</param>
    /// <param name="tag">An optional ASN.1 tag to be applied to the serialized object.</param>
    /// <returns>The number of bytes written.</returns>
    public static int SerializeCer(object element, Span<byte> destination, Asn1Tag? tag = null)
    {
        return Serialize(element, destination, AsnEncodingRules.CER, tag);
    }

    /// <summary>
    /// Serializes an object using DER encoding rules with an optional <paramref name="tag"/>.
    /// </summary>
    /// <param name="element">The object to serialize.</param>
    /// <param name="tag">An optional ASN.1 tag to be applied to the serialized object.</param>
    /// <returns>The encoded data as a byte array.</returns>
    public static byte[] SerializeDer(object element, Asn1Tag? tag = null)
    {
        return Serialize(element, AsnEncodingRules.DER, tag);
    }

    /// <summary>
    /// Serializes an object to a given <paramref name="destination"/> using DER encoding rules with an optional <paramref name="tag"/>.
    /// </summary>
    /// <param name="element">The object to serialize.</param>
    /// <param name="destination">The buffer to write the encoded data to.</param>
    /// <param name="tag">An optional ASN.1 tag to be applied to the serialized object.</param>
    /// <returns>The number of bytes written.</returns>
    public static int SerializeDer(object element, Span<byte> destination, Asn1Tag? tag = null)
    {
        return Serialize(element, destination, AsnEncodingRules.DER, tag);
    }

    /// <summary>
    /// Deserializes the specified <paramref name="data"/> into an object of the given <paramref name="type"/> using the specified <paramref name="ruleSet"/> and optional <paramref name="tag"/>.
    /// </summary>
    /// <param name="data">The encoded data to deserialize.</param>
    /// <param name="type">The type to deserialize into.</param>
    /// <param name="ruleSet">The encoding rules for deserialization.</param>
    /// <param name="tag">An optional ASN.1 tag to be expected during deserialization.</param>
    /// <param name="options">Additional options for the reader.</param>
    /// <returns>The deserialized object.</returns>
    public static object Deserialize(ReadOnlyMemory<byte> data, Type type, AsnEncodingRules ruleSet, Asn1Tag? tag = null, AsnReaderOptions options = default)
    {
        var reader = new AsnReader(data, ruleSet, options);
        var value = new AsnSerializer().Deserialize(reader, type, tag);

        reader.ThrowIfNotEmpty();

        return value;
    }

    /// <summary>
    /// Deserializes data from the specified <see cref="AsnReader"/> into an object of the given <paramref name="type"/> with an optional <paramref name="expectedTag"/>.
    /// </summary>
    /// <param name="reader">The reader to deserialize from.</param>
    /// <param name="type">The type to deserialize into.</param>
    /// <param name="expectedTag">An optional expected ASN.1 tag.</param>
    /// <returns>The deserialized object.</returns>
    public object Deserialize(AsnReader reader, Type type, Asn1Tag? expectedTag = null)
    {
        var converter = _converterResolver.Resolve(type, out var resolvedType);
        return Deserialize(reader, resolvedType, converter, expectedTag);
    }

    /// <summary>
    /// Deserializes data from the specified <see cref="AsnReader"/> into an object of the given <paramref name="asnPropertyInfo"/>.
    /// </summary>
    /// <param name="reader">The reader to deserialize from.</param>
    /// <param name="asnPropertyInfo">The ASN.1 property information.</param>
    /// <returns>The deserialized object.</returns>
    public object Deserialize(AsnReader reader, AsnPropertyInfo asnPropertyInfo)
    {
        var converter = _converterResolver.Resolve(asnPropertyInfo, out var resolvedType);
        return Deserialize(reader, resolvedType, converter, asnPropertyInfo.Tag);
    }

    /// <summary>
    /// Deserializes data from the specified <see cref="AsnReader"/> into an object of the given <paramref name="type"/> using the provided <paramref name="expectedTag"/> and optional custom converter.
    /// </summary>
    /// <param name="reader">The reader to deserialize from.</param>
    /// <param name="expectedTag">The expected tag for the ASN.1 data.</param>
    /// <param name="type">The type to deserialize into.</param>
    /// <param name="converter">An optional custom converter to use during deserialization.</param>
    /// <returns>The deserialized object.</returns>
    public object Deserialize(AsnReader reader, Type type, AsnConverter converter, Asn1Tag? expectedTag = null)
    {
        return converter.Read(reader, expectedTag, type, this);
    }

    /// <summary>
    /// Deserializes the specified <paramref name="data"/> into an object of type <typeparamref name="T"/> using the specified <paramref name="ruleSet"/> and optional <paramref name="tag"/>.
    /// </summary>
    /// <typeparam name="T">The type to deserialize into.</typeparam>
    /// <param name="data">The encoded data to deserialize.</param>
    /// <param name="ruleSet">The encoding rules for deserialization.</param>
    /// <param name="expectedTag">An optional ASN.1 tag to be expected during deserialization.</param>
    /// <param name="options">Additional options for the reader.</param>
    /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
    public static T Deserialize<T>(ReadOnlyMemory<byte> data, AsnEncodingRules ruleSet, Asn1Tag? expectedTag = null, AsnReaderOptions options = default)
    {
        return (T)Deserialize(data, typeof(T), ruleSet, expectedTag, options);
    }

    /// <summary>
    /// Deserializes the specified <paramref name="data"/> into an object of the given <paramref name="type"/> using BER encoding rules with an optional <paramref name="tag"/>.
    /// </summary>
    /// <param name="data">The encoded data to deserialize.</param>
    /// <param name="type">The type to deserialize into.</param>
    /// <param name="expectedTag">An optional ASN.1 tag to be expected during deserialization.</param>
    /// <param name="options">Additional options for the reader.</param>
    /// <returns>The deserialized object.</returns>
    public static object DeserializeBer(ReadOnlyMemory<byte> data, Type type, Asn1Tag? expectedTag = null, AsnReaderOptions options = default)
    {
        return Deserialize(data, type, AsnEncodingRules.BER, expectedTag, options);
    }

    /// <summary>
    /// Deserializes the specified <paramref name="data"/> into an object of type <typeparamref name="T"/> using BER encoding rules with an optional <paramref name="tag"/>.
    /// </summary>
    /// <typeparam name="T">The type to deserialize into.</typeparam>
    /// <param name="data">The encoded data to deserialize.</param>
    /// <param name="expectedTag">An optional ASN.1 tag to be expected during deserialization.</param>
    /// <param name="options">Additional options for the reader.</param>
    /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
    public static T DeserializeBer<T>(ReadOnlyMemory<byte> data, Asn1Tag? expectedTag = null, AsnReaderOptions options = default)
    {
        return (T)DeserializeBer(data, typeof(T), expectedTag, options);
    }

    /// <summary>
    /// Deserializes the specified <paramref name="data"/> into an object of the given <paramref name="type"/> using CER encoding rules with an optional <paramref name="tag"/>.
    /// </summary>
    /// <param name="data">The encoded data to deserialize.</param>
    /// <param name="type">The type to deserialize into.</param>
    /// <param name="expectedTag">An optional ASN.1 tag to be expected during deserialization.</param>
    /// <param name="options">Additional options for the reader.</param>
    /// <returns>The deserialized object.</returns>
    public static object DeserializeCer(ReadOnlyMemory<byte> data, Type type, Asn1Tag? expectedTag = null, AsnReaderOptions options = default)
    {
        return Deserialize(data, type, AsnEncodingRules.CER, expectedTag, options);
    }

    /// <summary>
    /// Deserializes the specified <paramref name="data"/> into an object of type <typeparamref name="T"/> using CER encoding rules with an optional <paramref name="tag"/>.
    /// </summary>
    /// <typeparam name="T">The type to deserialize into.</typeparam>
    /// <param name="data">The encoded data to deserialize.</param>
    /// <param name="expectedTag">An optional ASN.1 tag to be expected during deserialization.</param>
    /// <param name="options">Additional options for the reader.</param>
    /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
    public static T DeserializeCer<T>(ReadOnlyMemory<byte> data, Asn1Tag? expectedTag = null, AsnReaderOptions options = default)
    {
        return (T)DeserializeCer(data, typeof(T), expectedTag, options);
    }

    /// <summary>
    /// Deserializes the specified <paramref name="data"/> into an object of the given <paramref name="type"/> using DER encoding rules with an optional <paramref name="tag"/>.
    /// </summary>
    /// <param name="data">The encoded data to deserialize.</param>
    /// <param name="type">The type to deserialize into.</param>
    /// <param name="expectedTag">An optional ASN.1 tag to be expected during deserialization.</param>
    /// <param name="options">Additional options for the reader.</param>
    /// <returns>The deserialized object.</returns>
    public static object DeserializeDer(ReadOnlyMemory<byte> data, Type type, Asn1Tag? expectedTag = null, AsnReaderOptions options = default)
    {
        return Deserialize(data, type, AsnEncodingRules.DER, expectedTag, options);
    }

    /// <summary>
    /// Deserializes the specified <paramref name="data"/> into an object of type <typeparamref name="T"/> using DER encoding rules with an optional <paramref name="tag"/>.
    /// </summary>
    /// <typeparam name="T">The type to deserialize into.</typeparam>
    /// <param name="data">The encoded data to deserialize.</param>
    /// <param name="expectedTag">An optional ASN.1 tag to be expected during deserialization.</param>
    /// <param name="options">Additional options for the reader.</param>
    /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
    public static T DeserializeDer<T>(ReadOnlyMemory<byte> data, Asn1Tag? expectedTag = null, AsnReaderOptions options = default)
    {
        return (T)DeserializeDer(data, typeof(T), expectedTag, options);
    }
}
