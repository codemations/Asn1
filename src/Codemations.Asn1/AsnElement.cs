using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using Codemations.Asn1.Extensions;

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

        /// <summary>
        /// Gets or sets the content encoded value.
        /// </summary>
        public ReadOnlyMemory<byte>? Value { get; set; }

        /// <summary>
        /// Gets the list of child elements.
        /// </summary>
        public IList<AsnElement> Elements { get; }

        public AsnElement(byte encodedTag) : this(encodedTag, new List<AsnElement>())
        {
        }

        public AsnElement(uint encodedTag) : this(encodedTag, new List<AsnElement>())
        {
        }

        public AsnElement(Asn1Tag tag): this(tag, new List<AsnElement>())
        {
        }

        public AsnElement(byte encodedTag, IEnumerable<AsnElement> elements) : this(encodedTag.ToAsn1Tag(), elements)
        {
        }

        public AsnElement(uint encodedTag, IEnumerable<AsnElement> elements) : this(encodedTag.ToAsn1Tag(), elements)
        {
        }

        public AsnElement(Asn1Tag tag, IEnumerable<AsnElement> elements) : this(tag, elements.ToList())
        {
        }

        public AsnElement(Asn1Tag tag, IList<AsnElement> elements)
        {
            this.Tag = tag;
            this.Elements = elements.ToList();
        }
    }
}
