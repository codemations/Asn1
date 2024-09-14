using System.Formats.Asn1;

namespace Codemations.Asn1
{


    /// <summary>
    ///   The class representing ASN.1 elements.
    /// </summary>
    public class AsnElement
    {
        /// <summary>
        /// Gets the tag identifying the content.
        /// </summary>
        public Asn1Tag Tag { get; }

        public AsnElement(Asn1Tag tag)
        {
            this.Tag = tag;
        }
    }
}
