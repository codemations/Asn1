using System;
using System.Formats.Asn1;

namespace Codemations.Asn1
{
    /// <summary>
    ///   The class representing ASN.1 elements.
    /// </summary>
    public abstract class AsnElement : IEquatable<AsnElement>
    {
        /// <summary>
        /// Gets the tag identifying the content.
        /// </summary>
        public Asn1Tag Tag { get; }

        protected AsnElement(Asn1Tag tag)
        {
            Tag = tag;
        }

        public override bool Equals(object? obj)
        {
            if(obj is AsnElement asnElement)
            {
                return Equals(asnElement);
            }
            return false;
        }

        public abstract bool Equals(AsnElement? asnElement);

        /// <summary>
        /// Overloads the equality operator to compare two AsnElement objects.
        /// </summary>
        /// <param name="left">The left AsnElement object.</param>
        /// <param name="right">The right AsnElement object.</param>
        /// <returns>True if the objects are equal, otherwise false.</returns>
        public static bool operator ==(AsnElement? left, AsnElement? right)
        {
            if (left is null)
            {
                return right is null;
            }
            return left.Equals(right);
        }

        /// <summary>
        /// Overloads the inequality operator to compare two AsnElement objects.
        /// </summary>
        /// <param name="left">The left AsnElement object.</param>
        /// <param name="right">The right AsnElement object.</param>
        /// <returns>True if the objects are not equal, otherwise false.</returns>
        public static bool operator !=(AsnElement? left, AsnElement? right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Tag);
            AddHashValues(ref hash);
            return hash.ToHashCode();
        }

        internal abstract void AddHashValues(ref HashCode hash);
    }
}
