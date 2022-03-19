using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Numerics;
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
                AsnElement asnElement;
                var tag = reader.PeekTag();
                var value = reader.PeekContentBytes();

                if (tag.IsConstructed)
                {
                    var elements = Deserialize(value, ruleSet, options).ToList();
                    asnElement = new AsnConstructedElement(tag, elements);
                }
                else
                {
                    asnElement = new AsnPrimitiveElement(tag) {Value = value.ToArray()};
                }

                yield return asnElement;

                reader.ReadEncodedValue();
            }
        }

        /// <summary>
        ///   Serializes <paramref name="elements"/> with a given <paramref name="ruleSet"/>.
        /// </summary>
        /// <param name="elements">Elements to serialize.</param>
        /// <param name="ruleSet">The encoding constraints for the reader.</param>
        /// <returns>Encoded data.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="ruleSet"/> is not defined.
        /// </exception>
        public static byte[] Serialize(IEnumerable<AsnElement> elements, AsnEncodingRules ruleSet)
        {
            var writer = new AsnWriter(ruleSet);
            Serialize(writer, elements);
            return writer.Encode();
        }

        private static void Serialize(AsnWriter writer, IEnumerable<AsnElement> elements)
        {
            foreach (var element in elements)
            {
                switch (element)
                {
                    case AsnConstructedElement constructedElement:
                        writer.PushSequence(constructedElement.Tag);
                        Serialize(writer, constructedElement.Elements);
                        writer.PopSequence(constructedElement.Tag);
                        break;

                    case AsnPrimitiveElement {Value: null}:
                        writer.WriteNull(element.Tag);
                        break;

                    case AsnPrimitiveElement primitiveElement:
                        writer.WriteOctetString(primitiveElement.Value?.ToArray(), primitiveElement.Tag);
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

        private static void Serialize(AsnWriter writer, object element)
        {
            foreach (var property in element.GetType().GetProperties())
            {
                if (property.GetCustomAttribute(typeof(Asn1TagAttribute)) is Asn1TagAttribute asn1TagAttribute &&
                    property.GetValue(element) is { } subElement)
                {
                    var tag = asn1TagAttribute.Tag;
                    if (tag.IsConstructed)
                    {
                        writer.PushSequence(tag);
                        Serialize(writer, subElement);
                        writer.PopSequence(tag);
                    }
                    else
                    {
                        Write(writer, subElement, tag);
                    }
                }
            }
        }

        public static T Deserialize<T>(ReadOnlyMemory<byte> data, AsnEncodingRules ruleSet, AsnReaderOptions options = default) where T : class, new()
        {
            var reader = new AsnReader(data, ruleSet);
            var deserialized = new T();
            Deserialize(reader, deserialized);
            return deserialized;
        }

        private static void Deserialize(AsnReader reader, object element)
        {
            while (reader.HasData)
            {
                var asn1Tag = reader.PeekTag();
                foreach (var property in element.GetType().GetProperties())
                {
                    if (property.GetCustomAttribute(typeof(Asn1TagAttribute)) is Asn1TagAttribute asn1TagAttribute &&
                        asn1Tag.TagClass == asn1TagAttribute.Tag.TagClass &&
                        asn1Tag.TagValue == asn1TagAttribute.Tag.TagValue &&
                        asn1Tag.IsConstructed == asn1TagAttribute.Tag.IsConstructed)

                    {
                        if (asn1Tag.IsConstructed)
                        {
                            var subElement = Activator.CreateInstance(property.PropertyType)!;
                            property.SetValue(element, subElement);
                            Deserialize(reader.ReadSequence(asn1Tag), subElement);
                        }
                        else
                        {
                            var value = Read(reader, property.PropertyType, asn1Tag);
                            property.SetValue(element, value);
                        }
                    }
                }
            }
        }

        private static object Read(AsnReader reader, Type type, Asn1Tag tag)
        {
            if (type == typeof(byte[]))
            {
                return reader.ReadOctetString(tag);
            }
            if (type == typeof(long?))
            {
                return (long)reader.ReadInteger(tag);
            }

            throw new NotImplementedException($"Type '{type.Name}' is not handled yet");
        }

        public static Asn1Tag? GetAsn1Tag(PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetCustomAttribute(typeof(Asn1TagAttribute)) is Asn1TagAttribute asn1TagAttribute)
            {
                return asn1TagAttribute.Tag;
            }

            return null;
        }

        private static readonly Type[] IntegerTypes =
        {
        typeof(long), typeof(long?),
        typeof(ulong), typeof(ulong?),
        typeof(int), typeof(int?),
        typeof(uint), typeof(uint?),
        typeof(short), typeof(short?),
        typeof(ushort), typeof(ushort?),
        typeof(sbyte), typeof(sbyte?),
        typeof(byte), typeof(byte?)
    };

        private static void Write(AsnWriter writer, object element, Asn1Tag tag)
        {
            var type = element.GetType();
            if (element.GetType() == typeof(byte[]))
            {
                writer.WriteOctetString(element as byte[], tag);
            }
            else if (IntegerTypes.Contains(type))
            {
                writer.WriteInteger((long)element, tag);
            }
            else if (type.IsEnum ||
                     (type.IsGenericType && (type.GenericTypeArguments.SingleOrDefault()?.IsEnum ?? false)))
            {
                writer.WriteEnumeratedValue((Enum)element, tag);
            }
            else
            {
                throw new NotImplementedException($"Type '{type.Name}' is not handled yet");
            }
        }
    }
}
