using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using Codemations.Asn1.Extensions;

namespace Codemations.Asn1;

public partial class AsnSerializer
{
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
}
