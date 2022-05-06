using System.Collections.Generic;
using System.Formats.Asn1;
using Xunit;

namespace Codemations.Asn1.Tests
{
    public class AsnExtensionsTests
    {
        public static IEnumerable<object[]> DataByte
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

        public static IEnumerable<object[]> DataInt
        {
            get
            {
                yield return new object[] { new Asn1Tag(TagClass.ContextSpecific, 0x2000, false), 0x2F80 };
                yield return new object[] { new Asn1Tag(TagClass.ContextSpecific, 0x101F, false), 0x9F9F };
                yield return new object[] { new Asn1Tag(TagClass.ContextSpecific, 0x1000, true), 0x9FA0 };
                yield return new object[] { new Asn1Tag(TagClass.ContextSpecific, 0x101F, true), 0x9FBF };
                yield return new object[] { new Asn1Tag(TagClass.Universal, 0x100B, true), 0x9F2B };
                yield return new object[] { new Asn1Tag(TagClass.Universal, 0x1000, false), 0x9F00 };
                yield return new object[] { new Asn1Tag(TagClass.Universal, 0x101F, false), 0x9F1F };
                yield return new object[] { new Asn1Tag(TagClass.Private, 0x1000, true), 0x9FE0 };
                yield return new object[] { new Asn1Tag(TagClass.Private, 0x101F, true), 0x9FFF };
            }
        }

        [Theory]
        [MemberData(nameof(DataByte))]
        public void ShouldConvertTagToByte(Asn1Tag asn1Tag, byte expectedTag)
        {
            // Act
            var actualTag = asn1Tag.ToByte();

            // Assert
            Assert.Equal(expectedTag, actualTag);
        }

        [Theory]
        [MemberData(nameof(DataInt))]
        public void ShouldConvertTagToInt(Asn1Tag asn1Tag, int expectedTag)
        {
            // Act
            var actualTag = asn1Tag.ToInt();

            // Assert
            Assert.Equal(expectedTag, actualTag);
        }

        [Theory]
        [MemberData(nameof(DataByte))]
        public void ShouldConvertByteToTag(Asn1Tag expectedAsn1Tag, byte tag)
        {
            // Act
            var actualAsn1Tag = tag.ToAsn1Tag();

            // Assert
            Assert.Equal(expectedAsn1Tag, actualAsn1Tag);
        }

        [Theory]
        [MemberData(nameof(DataInt))]
        public void ShouldConvertIntToTag(Asn1Tag expectedAsn1Tag, int tag)
        {
            // Act
            var actualAsn1Tag = tag.ToAsn1Tag();

            // Assert
            Assert.Equal(expectedAsn1Tag, actualAsn1Tag);
        }

        [Fact]
        public void ShouldConvertIntToTag1()
        {
            var a = new Asn1Tag(TagClass.ContextSpecific, 0x2000, false);

            // Act
            var actualAsn1Tag = 0x2F80.ToAsn1Tag();

            // Assert
            Assert.Equal(a, actualAsn1Tag);
        }
    }
}