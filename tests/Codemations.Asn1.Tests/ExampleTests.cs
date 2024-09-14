using System.Formats.Asn1;
using Codemations.Asn1.Attributes;
using Codemations.Asn1.Converters;
using NUnit.Framework;

namespace Codemations.Asn1.Tests;

public class ExampleTests
{
    public class FooQuestion
    {
        public int TrackingNumber { get; set; }

        [AsnConverter(typeof(AsnUtf8StringConverter))]
        public string? Question { get; set; }
    }

    [Test]
    public void ShouldSerializeExample()
    {
        // Arrange
        var myQuestion = new FooQuestion
        {
            TrackingNumber = 5,
            Question = "Anybody there?"
        };
        var expectedData = new byte[] {
            0x30, 0x13, 
                0x02, 0x01, 0x05, 
                0x0C, 0x0e, 0x41, 0x6e, 0x79, 0x62, 0x6f, 0x64, 0x79, 0x20, 0x74, 0x68, 0x65, 0x72, 0x65, 0x3f
        };

        // Act
        var actualData = AsnSerializer.Serialize(myQuestion, AsnEncodingRules.DER);

        // Assert
        Assert.That(actualData, Is.EqualTo(expectedData));
    }

    [Test]
    public void ShouldDeserializeExample()
    {
        // Arrange
        var expectedQuestion = new FooQuestion
        {
            TrackingNumber = 5,
            Question = "Anybody there?"
        };
        var encodedData = new byte[] {
            0x30, 0x13, 
                0x02, 0x01, 0x05, 
                0x0C, 0x0e, 0x41, 0x6e, 0x79, 0x62, 0x6f, 0x64, 0x79, 0x20, 0x74, 0x68, 0x65, 0x72, 0x65, 0x3f
        };

        // Act
        var actualQuestion = AsnSerializer.Deserialize<FooQuestion>(encodedData, AsnEncodingRules.DER);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actualQuestion.TrackingNumber, Is.EqualTo(expectedQuestion.TrackingNumber));
            Assert.That(actualQuestion.Question, Is.EqualTo(expectedQuestion.Question));
        });
    }
}