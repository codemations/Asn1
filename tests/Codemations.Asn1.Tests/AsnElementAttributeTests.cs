using System.Formats.Asn1;
using Codemations.Asn1.Attributes;
using NUnit.Framework;

namespace Codemations.Asn1.Tests;

public class AsnElementAttributeTests : TagTestData
{
    [TestCaseSource(nameof(ByteData))]
    public void ShouldCreateTagFromByte(Asn1Tag expectedTag, byte encodedTag)
    {
        // Act
        var attribute = new AsnTagAttribute(encodedTag);

        // Assert
        Assert.That(attribute.Tag, Is.EqualTo(expectedTag));
    }

    [TestCaseSource(nameof(UIntData))]
    public void ShouldCreateTagFromUInt(Asn1Tag expectedTag, uint encodedTag)
    {
        // Act
        var attribute = new AsnTagAttribute(encodedTag);

        // Assert
        Assert.That(attribute.Tag, Is.EqualTo(expectedTag));
    }

    [Test]
    public void ShouldCreateTagFromByteArray()
    {
        // Arrange
        var expectedTag = new Asn1Tag(TagClass.ContextSpecific, 0x1F, false);
        var encodedTag = new byte[] { 0x9F, 0x1F };

        // Act
        var attribute = new AsnTagAttribute(encodedTag);

        // Assert
        Assert.That(attribute.Tag, Is.EqualTo(expectedTag));
    }

    [Test]
    public void ShouldCreateTagFromValue()
    {
        // Arrange
        var expectedTag = Asn1Tag.Integer;

        // Act
        var attribute = new AsnTagAttribute(TagClass.Universal, 2, false);

        // Assert
        Assert.That(attribute.Tag, Is.EqualTo(expectedTag));
    }

    [Test]
    public void ShouldCreateTagFromUniversalTag()
    {
        // Arrange
        var expectedTag = Asn1Tag.Integer;

        // Act
        var attribute = new AsnTagAttribute(UniversalTagNumber.Integer, false);

        // Assert
        Assert.That(attribute.Tag, Is.EqualTo(expectedTag));
    }
}