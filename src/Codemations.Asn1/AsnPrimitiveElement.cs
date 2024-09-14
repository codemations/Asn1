using System;
using System.Formats.Asn1;

namespace Codemations.Asn1
{
    /// <summary>
    ///   The class representing ASN.1 primitive elements.
    /// </summary>
    public class AsnPrimitiveElement : AsnElement
    {
        /// <summary>
        /// Gets or sets the content encoded value.
        /// </summary>
        public ReadOnlyMemory<byte> Value { get; }

        public AsnPrimitiveElement(Asn1Tag tag, ReadOnlyMemory<byte> value) : base(tag)
        {
            Value = value;
        }
    }
}
