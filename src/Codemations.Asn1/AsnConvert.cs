using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;
using Codemations.Asn1.TypeConverters;

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

            if (element.GetType().GetCustomAttribute<AsnChoiceAttribute>() is not null)
            {
                new AsnChoiceConverter().Write(writer, element);
            }
            else
            {
                new AsnSequenceConverter().Write(writer, element);
            }

            return writer.Encode();
        }

        public static T Deserialize<T>(ReadOnlyMemory<byte> data, AsnEncodingRules ruleSet, AsnReaderOptions options = default) where T : class, new()
        {
            var reader = new AsnReader(data, ruleSet, options);

            var deserialized = typeof(T).GetCustomAttribute<AsnChoiceAttribute>() is not null ? 
                new AsnChoiceConverter().Read(reader, typeof(T)) : 
                new AsnSequenceConverter().Read(reader, typeof(T));

            if (reader.HasData)
            {
                throw new AsnConversionException("Not read data left.");
            }

            return (deserialized as T)!;
        }
    }
}
