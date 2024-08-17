using System.Formats.Asn1;
using System.Numerics;
using NUnit.Framework;

namespace Codemations.Asn1.Tests
{
    public class ExampleTests
    {
        [AsnSequence]
        public class FooQuestion
        {
            [AsnElement]
            public BigInteger TrackingNumber { get; set; }
            [AsnElement]
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
                    0x16, 0x0e, 0x41, 0x6e, 0x79, 0x62, 0x6f, 0x64, 0x79, 0x20, 0x74, 0x68, 0x65, 0x72, 0x65, 0x3f
            };

            // Act
            var actualData = AsnConvert.Serialize(myQuestion, AsnEncodingRules.DER);

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
            var data = new byte[] {
                0x30, 0x13, 
                    0x02, 0x01, 0x05, 
                    0x16, 0x0e, 0x41, 0x6e, 0x79, 0x62, 0x6f, 0x64, 0x79, 0x20, 0x74, 0x68, 0x65, 0x72, 0x65, 0x3f
            };

            // Act
            var actualQuestion = AsnConvert.Deserialize<FooQuestion>(data, AsnEncodingRules.DER);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(actualQuestion.TrackingNumber, Is.EqualTo(expectedQuestion.TrackingNumber));
                Assert.That(actualQuestion.Question, Is.EqualTo(expectedQuestion.Question));
            });
        }
    }
}