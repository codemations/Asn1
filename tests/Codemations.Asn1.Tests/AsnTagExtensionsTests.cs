using System;
using System.Formats.Asn1;
using Codemations.Asn1.Extensions;
using NUnit.Framework;

namespace Codemations.Asn1.Tests
{
    public class AsnTagExtensionsTests : TagTestData
    {
        [TestCaseSource(nameof(ByteData))]
        public void ShouldConvertTagToByte(Asn1Tag asn1Tag, byte expectedTag)
        {
            // Act
            var actualTag = asn1Tag.ToByte();

            // Assert
            Assert.That(actualTag, Is.EqualTo(expectedTag));
        }

        [TestCaseSource(nameof(ByteData))]
        public void ShouldConvertByteToTag(Asn1Tag expectedAsn1Tag, byte tag)
        {
            // Act
            var actualAsn1Tag = tag.ToAsn1Tag();

            // Assert
            Assert.That(actualAsn1Tag, Is.EqualTo(expectedAsn1Tag));
        }


        [TestCaseSource(nameof(ByteData))]
        [TestCaseSource(nameof(UIntData))]
        public void ShouldConvertTagToUInt(Asn1Tag asn1Tag, uint expectedTag)
        {
            // Act
            var actualTag = asn1Tag.ToUInt();
        
            // Assert
            Assert.That(actualTag, Is.EqualTo(expectedTag));
        }

        [TestCaseSource(nameof(ByteData))]
        [TestCaseSource(nameof(UIntData))]
        public void ShouldConvertUIntToTag(Asn1Tag expectedAsn1Tag, uint tag)
        {
            // Act
            var actualAsn1Tag = tag.ToAsn1Tag();

            // Assert
            Assert.That(actualAsn1Tag, Is.EqualTo(expectedAsn1Tag));
        }

        [Test]
        public void ToByte_OutOfRangeValue_ShouldThrow()
        {
            // Arrange
            var asn1Tag = new Asn1Tag(TagClass.ContextSpecific, MaxByteEncodedTagValue + 1);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => asn1Tag.ToByte());
        }

        [Test]
        public void ToUInt_OutOfRangeValue_ShouldThrow()
        {
            // Arrange
            var asn1Tag = new Asn1Tag(TagClass.ContextSpecific, MaxUIntEncodedTagValue + 1);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => asn1Tag.ToUInt());
        }
    }
}