using System;
using System.Collections.Generic;
using System.Formats.Asn1;

namespace Codemations.Asn1
{
    /// <summary>
    ///   Provides methods for converting between .NET types and ASN.1 encoded values.
    /// </summary>
    public class AsnNode
    {
        /// <summary>
        /// Gets or sets the tag identifying the content.
        /// </summary>
        public Asn1Tag Tag { get; set; }

        /// <summary>
        /// Gets or sets the content encoded value.
        /// </summary>
        public ReadOnlyMemory<byte> Value { get; set; }

        /// <summary>
        /// Gets or sets the list of child nodes.
        /// </summary>
        public IList<AsnNode>? Nodes { get; set; }
    }
}
