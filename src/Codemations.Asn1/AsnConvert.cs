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
                var asnNode = new AsnNode { Tag = reader.PeekTag(), Value = reader.PeekContentBytes().ToArray() };

                if (asnNode.Tag.IsConstructed)
                {
                    asnNode.Nodes = Deserialize(asnNode.Value, ruleSet, options).ToList();
                }

                yield return asnNode;

                reader.ReadEncodedValue();
            }
        }

        /// <summary>
        ///   Serializes <paramref name="nodes"/> with a given <paramref name="ruleSet"/>.
        /// </summary>
        /// <param name="nodes">Nodes to serialize.</param>
        /// <param name="ruleSet">The encoding constraints for the reader.</param>
        /// <returns>Encoded data.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="ruleSet"/> is not defined.
        /// </exception>
        public static byte[] Serialize(IEnumerable<AsnNode> nodes, AsnEncodingRules ruleSet)
        {
            var writer = new AsnWriter(ruleSet);
            Serialize(writer, nodes);
            return writer.Encode();
        }

        private static void Serialize(AsnWriter writer, IEnumerable<AsnNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.Tag.IsConstructed)
                {
                    writer.PushSequence(node.Tag);
                    if (node.Nodes is not null)
                    {
                        Serialize(writer, node.Nodes);
                    }
                    writer.PopSequence(node.Tag);
                }
                else
                {
                    if (node.Value is null)
                    {
                        writer.WriteNull(node.Tag);
                    }
                    else
                    {
                        writer.WriteOctetString(node.Value?.ToArray(), node.Tag);
                    }
                }
            }
        }
    }
}
