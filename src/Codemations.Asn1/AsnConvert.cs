using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;

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
    }
}
