using System.Formats.Asn1;

namespace Codemations.Asn1
{
    /// <summary>
    ///   The base class for ASN.1 element classes.
    /// </summary>
    public abstract class AsnElement
    {
        /// <summary>
        /// Gets the tag identifying the content.
        /// </summary>
        public Asn1Tag Tag { get; }

        protected AsnElement(Asn1Tag tag)
        {
            this.Tag = tag;
        }
    }
}
