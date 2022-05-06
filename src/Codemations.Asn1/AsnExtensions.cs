using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;

namespace Codemations.Asn1
{
    /// <summary>
    ///   <see cref="System.Formats.Asn1"/> extension methods.
    /// </summary>
    public static class AsnExtensions
    {
        private const byte ClassMask = 0b1100_0000;
        private const byte ConstructedMask = 0b0010_0000;
        private const int TagNumberMask = 0x7070701F;
        private const uint UpperBytesMask = 0x8F8F8F00;
        private const byte UpperByteMask = 0b1000_1111;

        /// <summary>
        ///   Converts <see cref="Asn1Tag"/> to value encoded as <see cref="int"/>.
        /// </summary>
        /// <returns>Encoded tag.</returns>
        public static int ToInt(this Asn1Tag tag)
        {

            var bytes = BitConverter.GetBytes(tag.TagValue);
            var firstByte = bytes.First();
            var upperBytes = bytes.Skip(1).Reverse().SkipWhile(x=>x == 0x00).Reverse();

            firstByte = (byte)((int)tag.TagClass | (tag.IsConstructed ? ConstructedMask : 0) | firstByte);
            upperBytes = upperBytes.Select(x => (byte)(x | UpperByteMask));

            var resultByteList = new List<byte>();
            resultByteList.Add(firstByte);
            resultByteList.AddRange(upperBytes);

            var missingBytes = sizeof(int) - resultByteList.Count;
            resultByteList.AddRange(new byte[missingBytes]);

            return BitConverter.ToInt32(resultByteList.ToArray());
        }

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
            return ToAsn1Tag(tag);
        }

        /// <summary>
        ///   Converts tag encoded as <see cref="int"/> to <see cref="Asn1Tag"/>.
        /// </summary>
        /// <returns><see cref="Asn1Tag"/>.</returns>
        public static Asn1Tag ToAsn1Tag(this int tag)
        {
            return new Asn1Tag((TagClass)(tag & ClassMask), tag & TagNumberMask, (tag & ConstructedMask) != 0); ;
        }
    }
}