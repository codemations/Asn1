using System;
using System.Collections;
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

        internal static void Serialize(AsnWriter writer, object item)
        {
            if (item.GetType().GetCustomAttribute<AsnChoiceAttribute>() is not null)
            {
                SerializeChoice(writer, item);
            }
            else
            {
                SerializeSequence(writer, item);
            }
        }

        private static void SerializeChoice(AsnWriter writer, object item)
        {
            var propertyInfos = item.GetPropertyInfos<AsnElementAttribute>().ToArray();

            switch (propertyInfos.Length)
            {
                case 0:
                    throw new AsnConversionException("No choice element to serialize.");

                case 1:
                    SerializeElement(writer, item, propertyInfos.Single());
                    break;

                default:
                    throw new AsnConversionException("Multiple non-null choice elements.");
            }
        }

        private static void SerializeSequence(AsnWriter writer, object item)
        {
            foreach (var propertyInfo in item.GetPropertyInfos<AsnElementAttribute>())
            {
                SerializeElement(writer, item, propertyInfo);
            }
        }

        private static void SerializeElement(AsnWriter writer, object item, PropertyInfo propertyInfo)
        {
            var asnElementAttribute = propertyInfo.GetCustomAttribute<AsnElementAttribute>()!;
            var value = propertyInfo.GetValue(item)!;
            var tag = asnElementAttribute.Tag;

            if (IsEnumerable(value))
            {
                writer.PushSequence(tag);
                foreach (var element in (value as IEnumerable)!)
                {
                    SerializeElement(writer, null, element, asnElementAttribute.Converter);
                }
                writer.PopSequence(tag);
            }
            else
            {
                SerializeElement(writer, tag, value, asnElementAttribute.Converter);
            }
        }

        private static bool IsEnumerable(object value)
        {
            return IsEnumerable(value.GetType());
        }


        private static bool IsEnumerable(Type type)
        {
            return type != typeof(string) && type != typeof(byte[]) && typeof(IEnumerable).IsAssignableFrom(type);
        }

        private static void SerializeElement(AsnWriter writer, Asn1Tag? tag, object value, IAsnConverter converter)
        {
            converter.Write(writer, tag, value);
        }

        public static T Deserialize<T>(ReadOnlyMemory<byte> data, AsnEncodingRules ruleSet, AsnReaderOptions options = default) where T : class, new()
        {
            var reader = new AsnReader(data, ruleSet, options);
            var deserialized = new T();
            Deserialize(reader, deserialized);

            if (reader.HasData)
            {
                throw new AsnConversionException("Not read data left.");
            }

            return deserialized;
        }

        internal static void Deserialize(AsnReader reader, object item)
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
            var propertyInfos = item.GetPropertyInfos<AsnElementAttribute>(true)
                .Where(x => x.GetCustomAttribute<AsnElementAttribute>()!.Tag == tag).ToArray();

            switch (propertyInfos.Length)
            {
                case 0:
                    throw new AsnConversionException("No choice element with given tag.", tag);

                case 1:
                    DeserializeElement(reader, item, propertyInfos.Single());
                    break;

                default:
                    throw new AsnConversionException("Multiple choice elements with given tag.", tag);
            }
        }

        private static void DeserializeSequence(AsnReader reader, object item)
        {
            foreach (var propertyInfo in item.GetPropertyInfos<AsnElementAttribute>(true))
            {
                var asnElementAttribute = propertyInfo.GetCustomAttribute<AsnElementAttribute>()!;

                try
                {
                    DeserializeElement(reader, item, propertyInfo);
                }
                catch (Exception e)
                {
                    if (!asnElementAttribute.Optional)
                    {
                        throw new AsnConversionException("Value for required element is missing.", asnElementAttribute.Tag, e);
                    }
                }
            }
        }

        private static void DeserializeElement(AsnReader reader, object item, PropertyInfo propertyInfo)
        {
            var asnElementAttribute = propertyInfo.GetCustomAttribute<AsnElementAttribute>()!;
            object value;

            if (IsEnumerable(propertyInfo.PropertyType))
            {
                var type = propertyInfo.PropertyType.GetGenericArguments().Single();
                value = DeserializeElements(reader, asnElementAttribute.Tag, type, asnElementAttribute.Converter);
            }
            else
            {
                value = DeserializeElement(reader, asnElementAttribute.Tag, propertyInfo.PropertyType, asnElementAttribute.Converter);
            }

            propertyInfo.SetValue(item, value);
        }

        private static IList DeserializeElements(AsnReader reader, Asn1Tag tag, Type type, IAsnConverter converter)
        {
            var sequenceReader = reader.ReadSequence(tag);
            var sequence = (Activator.CreateInstance(typeof(List<>).MakeGenericType(type)) as IList)!;
            while (sequenceReader.HasData)
            {
                sequence.Add(DeserializeElement(sequenceReader, null, type, converter));
            }
            return sequence;
        }

        private static object DeserializeElement(AsnReader reader, Asn1Tag? tag, Type type, IAsnConverter converter)
        {
            return converter.Read(reader, tag, type);
        }
    }
}
