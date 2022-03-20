using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;

namespace Codemations.Asn1
{
    /// <summary>
    ///   The class representing ASN.1 constructed elements.
    /// </summary>
    public class AsnConstructedElement : AsnElement
    {
        /// <summary>
        /// Gets or sets the list of child elements.
        /// </summary>
        public IEnumerable<AsnElement> Elements { get; set; }

        /// <summary>
        ///   Create an <see cref="AsnConstructedElement"/> for a given <paramref name="tag"/>.
        /// </summary>
        /// <param name="tag">
        ///   The tag identifying the <see cref="AsnConstructedElement"/>.
        /// </param>
        public AsnConstructedElement(Asn1Tag tag) : this(tag, new List<AsnElement>())
        {
        }

        /// <summary>
        ///   Create an <see cref="AsnConstructedElement"/> for a given <paramref name="tag"/>.
        /// </summary>
        /// <param name="tag">
        ///   The tag identifying the <see cref="AsnConstructedElement"/>.
        /// </param>
        /// <param name="elements">
        ///   An enumerable of child elements.
        /// </param>
        public AsnConstructedElement(Asn1Tag tag, IEnumerable<AsnElement> elements) : base(tag)
        {
            this.Elements = elements;
        }
    }
}