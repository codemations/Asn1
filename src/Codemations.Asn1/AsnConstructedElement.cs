using System;
using System.Collections;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;

namespace Codemations.Asn1
{
    /// <summary>
    /// Represents a constructed ASN.1 element, which decorates an underlying list of ASN.1 elements.
    /// </summary>
    public sealed class AsnConstructedElement : AsnElement, IEnumerable<AsnElement>
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

        public void Add(AsnElement item)
        {
            _elements.Add(item);
        }

        public override bool Equals(AsnElement? asnElement)
        {
            if(asnElement is AsnConstructedElement asnConstructedElement)
            {
                return Equals(asnConstructedElement);
            }
            return false;
        }

        private bool Equals(AsnConstructedElement? asnConstructedElement)
        {
            if(asnConstructedElement is null)
            {  
                return false; 
            }
            return Tag.Equals(asnConstructedElement.Tag) && _elements.SequenceEqual(asnConstructedElement._elements);
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

        internal override void AddHashValues(ref HashCode hash)
        {
            foreach (var element in _elements)
            {
                hash.Add(element);
            }
        }
    }
}
