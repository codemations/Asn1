using System.Formats.Asn1;
using Xunit;

namespace Codemations.Asn1.Tests
{
    public class AsnElementAttributeTests : TagTestData
    {
        [Theory]
        [MemberData(nameof(UIntData))]
        public void ShouldCreateTagFromUInt(Asn1Tag expectedTag, uint encodedTag)
        {
            // Act
            var attribute = new AsnElementAttribute(encodedTag);

            // Assert
            Assert.Equal(expectedTag, attribute.Tag);
        }
    }
}