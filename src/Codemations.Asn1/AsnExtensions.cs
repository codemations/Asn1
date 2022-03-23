using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;

namespace Codemations.Asn1
{
    /// <summary>
    ///   <see cref="System.Formats.Asn1"/> extension methods.
    /// </summary>
    public static class AsnExtensions
    {
        private const byte ClassMask = 0b1100_0000;
        private const byte ConstructedMask = 0b0010_0000;
        private const byte TagNumberMask = 0b0001_1111;

        /// <summary>
        ///   Converts <see cref="Asn1Tag"/> to value encoded as <see cref="byte"/>.
        /// </summary>
        /// <returns>Encoded tag.</returns>
        public static byte ToByte(this Asn1Tag tag)
        {
            return (byte)((int)tag.TagClass | (tag.IsConstructed ? ConstructedMask : 0) | tag.TagValue);
        }

        /// <summary>
        ///   Converts tag encoded as <see cref="byte"/> to <see cref="Asn1Tag"/>.
        /// </summary>
        /// <returns><see cref="Asn1Tag"/>.</returns>
        public static Asn1Tag ToAsn1Tag(this byte tag)
        {
            return new Asn1Tag((TagClass)(tag & ClassMask), tag & TagNumberMask, (tag & ConstructedMask) != 0);
        }

        /// <summary>
        ///   Converts tag encoded as <see cref="int"/> to <see cref="Asn1Tag"/>.
        /// </summary>
        /// <returns><see cref="Asn1Tag"/>.</returns>
        public static Asn1Tag ToAsn1Tag(this int tag)
        {
            if (tag > byte.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(tag), tag, 
                    "Given value is greater than 0xFF which is the highest valid ASN.1 tag value");
            }
            return ToAsn1Tag((byte)tag);
        }

        internal static IEnumerable<PropertyInfo> GetPropertyInfos<T>(this object value, bool canBeNull = false) where T: Attribute
        {
            return value.GetType().GetProperties()
                .Where(x => x.GetCustomAttribute<T>() is not null)
                .Where(x => canBeNull || x.GetValue(value) is not null);
        }
    }
}