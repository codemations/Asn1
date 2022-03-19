using System.Formats.Asn1;

namespace Codemations.Asn1
{
    /// <summary>
    ///   The class representing ASN.1 primitive elements.
    /// </summary>
    public class AsnPrimitiveElement: AsnElement
    {
        /// <summary>
        /// Gets or sets the content encoded value.
        /// </summary>
        public byte[]? Value { get; set; }

        /// <summary>
        ///   Create an <see cref="AsnPrimitiveElement"/> for a given <paramref name="tag"/>.
        /// </summary>
        /// <param name="tag">
        ///   The tag identifying the <see cref="AsnPrimitiveElement"/>.
        /// </param>
        public AsnPrimitiveElement(Asn1Tag tag) : base(tag)
        {
        }
    }
}