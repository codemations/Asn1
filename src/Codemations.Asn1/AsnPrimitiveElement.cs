using System;
using System.Formats.Asn1;

namespace Codemations.Asn1
{
    /// <summary>
    ///   The class representing ASN.1 primitive elements.
    /// </summary>
    public sealed class AsnPrimitiveElement : AsnElement
    {
        /// <summary>
        /// Gets or sets the content encoded value.
        /// </summary>
        public ReadOnlyMemory<byte> Value { get; }

        public AsnPrimitiveElement(Asn1Tag tag) : this(tag, ReadOnlyMemory<byte>.Empty)
        {
        }

        public AsnPrimitiveElement(Asn1Tag tag, ReadOnlyMemory<byte> value) : base(tag)
        {
            Value = value;
        }

        public override bool Equals(AsnElement? asnElement)
        {
            if(asnElement is AsnPrimitiveElement asnPrimitiveElement)
            {
                return Equals(asnPrimitiveElement);
            }
            return false;
        }

        private bool Equals(AsnPrimitiveElement? asnPrimitiveElement)
        {
            if (asnPrimitiveElement is null)
            {
                return false;
            }
            return Tag.Equals(asnPrimitiveElement.Tag) && Value.Span.SequenceEqual(asnPrimitiveElement.Value.Span);
        }

        internal override void AddHashValues(ref HashCode hash)
        {
            foreach(var b in Value.Span)
            {
                hash.Add(b);
            }
        }
    }
}
