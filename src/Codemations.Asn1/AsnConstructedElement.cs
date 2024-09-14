using System.Collections;
using System.Collections.Generic;
using System.Formats.Asn1;

namespace Codemations.Asn1
{
    /// <summary>
    /// Represents a constructed ASN.1 element, which decorates an underlying list of ASN.1 elements.
    /// </summary>
    public class AsnConstructedElement : AsnElement, IEnumerable<AsnElement>
    {
        private readonly IList<AsnElement> _elements;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsnConstructedElement"/> class with an optional list of ASN.1 elements.
        /// </summary>
        /// <param name="elements">The list of ASN.1 elements to initialize the constructed element with. If null, a new list is created.</param>
        public AsnConstructedElement(Asn1Tag tag, IList<AsnElement>? elements = null) : base(tag)
        {
            _elements = elements ?? new List<AsnElement>();
        }

        /// <inheritdoc />
        public void Add(AsnElement item)
        {
            _elements.Add(item);
        }

        /// <inheritdoc />
        public IEnumerator<AsnElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
