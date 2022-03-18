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
        /// <returns>An iterator with AsnNode objects.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="ruleSet"/> is not defined.
        /// </exception>
        public static IEnumerable<AsnNode> Deserialize(ReadOnlyMemory<byte> data, AsnEncodingRules ruleSet, AsnReaderOptions options = default)
        {
            var reader = new AsnReader(data, ruleSet, options);
            while (reader.HasData)
            {
                var asnNode = new AsnNode { Tag = reader.PeekTag(), Value = reader.PeekContentBytes() };

                if (asnNode.Tag.IsConstructed)
                {
                    asnNode.Nodes = Deserialize(asnNode.Value, ruleSet, options).ToList();
                }

                yield return asnNode;

                reader.ReadEncodedValue();
            }
        }
    }
}
