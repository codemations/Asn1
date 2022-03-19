using System.Collections.Generic;
using System.Formats.Asn1;
using Xunit;

namespace Codemations.Asn1.Tests
{
    public class AsnExtensionsTests
    {
        public static IEnumerable<object[]> Data
        {
            get
            {
                yield return new object[] {new Asn1Tag(TagClass.ContextSpecific, 0x00, false), 0x80};
                yield return new object[] {new Asn1Tag(TagClass.ContextSpecific, 0x1F, false), 0x9F};
                yield return new object[] {new Asn1Tag(TagClass.ContextSpecific, 0x00, true), 0xA0};
                yield return new object[] {new Asn1Tag(TagClass.ContextSpecific, 0x1F, true), 0xBF};
                yield return new object[] {new Asn1Tag(TagClass.Universal, 0x00, false), 0x00};
                yield return new object[] {new Asn1Tag(TagClass.Universal, 0x1F, false), 0x1F};
                yield return new object[] {new Asn1Tag(TagClass.Private, 0x00, true), 0xE0};
                yield return new object[] {new Asn1Tag(TagClass.Private, 0x1F, true), 0xFF};
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ShouldConvertTagToByte(Asn1Tag asn1Tag, byte expectedTag)
        {
            // Act
            var actualTag = asn1Tag.ToByte();

            // Assert
            Assert.Equal(expectedTag, actualTag);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ShouldConvertByteToTag(Asn1Tag expectedAsn1Tag, byte tag)
        {
            // Act
            var actualAsn1Tag = tag.ToAsn1Tag();

            // Assert
            Assert.Equal(expectedAsn1Tag, actualAsn1Tag);
        }
    }
}