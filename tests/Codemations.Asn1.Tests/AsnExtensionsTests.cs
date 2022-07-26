using System.Formats.Asn1;
using Xunit;

namespace Codemations.Asn1.Tests
{
    public class AsnExtensionsTests : TagTestData
    {
        [Theory]
        [MemberData(nameof(ByteData))]
        public void ShouldConvertTagToByte(Asn1Tag asn1Tag, byte expectedTag)
        {
            // Act
            var actualTag = asn1Tag.ToByte();

            // Assert
            Assert.Equal(expectedTag, actualTag);
        }

        [Theory]
        [MemberData(nameof(ByteData))]
        public void ShouldConvertByteToTag(Asn1Tag expectedAsn1Tag, byte tag)
        {
            // Act
            var actualAsn1Tag = tag.ToAsn1Tag();

            // Assert
            Assert.Equal(expectedAsn1Tag, actualAsn1Tag);
        }


        [Theory]
        [MemberData(nameof(ByteData))]
        [MemberData(nameof(UIntData))]
        public void ShouldConvertTagToUInt(Asn1Tag asn1Tag, uint expectedTag)
        {
            // Act
            var actualTag = asn1Tag.ToUInt();
        
            // Assert
            Assert.Equal(expectedTag, actualTag);
        }

        [Theory]
        [MemberData(nameof(ByteData))]
        [MemberData(nameof(UIntData))]
        public void ShouldConvertUIntToTag(Asn1Tag expectedAsn1Tag, uint tag)
        {
            // Act
            var actualAsn1Tag = tag.ToAsn1Tag();

            // Assert
            Assert.Equal(expectedAsn1Tag, actualAsn1Tag);
        }
    }
}