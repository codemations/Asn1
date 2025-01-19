using Codemations.Asn1.Attributes;
using Codemations.Asn1.Extensions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Numerics;

namespace Codemations.Asn1.Tests
{
    internal partial class AsnSerializerTests
    {
        [Test]
        public void ShouldSerializeChoiceSequence()
        {
            var choice1 = new ChoiceElement
            {
                SequenceElement = new SequenceElement
                {
                    OctetString = new byte[] { 0xCA, 0xFE },
                    Integer = 10
                }
            };
            var choice1Encoded = new byte[] { 0xA0, 0x07, 0x81, 0x02, 0xCA, 0xFE, 0x82, 0x01, 0x0A };

            var choice2 = new ChoiceElement
            {
                BooleanElement = true
            };                  
            var choice2Encoded = new byte[] { 0x81, 0x01, 0xFF };

            var choiceSequence = new ChoiceSequence { Choice1 = choice1, Choice2 = choice2 };
            var choiceSequenceEncoded = new byte[] { 0x30, (byte)(choice1Encoded.Length + choice2Encoded.Length) }
                .Concat(choice1Encoded)
                .Concat(choice2Encoded)
                .ToArray();

            var actualEncodedValue = AsnSerializer.Serialize(choiceSequence, System.Formats.Asn1.AsnEncodingRules.BER);

            Assert.That(actualEncodedValue, Is.EqualTo(choiceSequenceEncoded));
        }

        [Test]
        public void ShouldDeserializeChoiceSequence()
        {
            var choice1 = new ChoiceElement
            {
                SequenceElement = new SequenceElement
                {
                    OctetString = new byte[] { 0xCA, 0xFE },
                    Integer = 10
                }
            };
            var choice1Encoded = new byte[] { 0xA0, 0x07, 0x81, 0x02, 0xCA, 0xFE, 0x82, 0x01, 0x0A };

            var choice2 = new ChoiceElement
            {
                BooleanElement = true
            };
            var choice2Encoded = new byte[] { 0x81, 0x01, 0xFF };

            var choiceSequence = new ChoiceSequence { Choice1 = choice1, Choice2 = choice2 };
            var choiceSequenceEncoded = new byte[] { 0x30, (byte)(choice1Encoded.Length + choice2Encoded.Length) }
                .Concat(choice1Encoded)
                .Concat(choice2Encoded)
                .ToArray();

            var actualChoiceSequence = AsnSerializer.Deserialize<ChoiceSequence>(choiceSequenceEncoded, AsnEncodingRules.BER);

            Assert.Multiple(() =>
            {
                Assert.That(actualChoiceSequence.Choice1?.SequenceElement?.Integer, Is.EqualTo(choiceSequence.Choice1.SequenceElement.Integer));
                Assert.That(actualChoiceSequence.Choice2?.BooleanElement, Is.EqualTo(choiceSequence.Choice2.BooleanElement));
            });
        }

        [Test]
        public void ShouldThrowWhenMultipleElementsWithTheSameTag()
        {
            // Arrange
            var expectedTag = new Asn1Tag(TagClass.ContextSpecific, 0);
            var choiceEncoded = new byte[] { expectedTag.ToByte(), 0x01, 0x00 };

            // Act & Assert
            var exception = Assert.Throws<AsnConversionException>(() => 
                AsnSerializer.DeserializeBer<InvalidChoiceElement>(choiceEncoded));
            Assert.That(exception.Tag, Is.EqualTo(expectedTag));
        }

        private static IEnumerable<TestCaseData> ChoiceModelData()
        {
            yield return new TestCaseData(
                new ChoiceElement
                {
                    SequenceElement = new SequenceElement
                    {
                        OctetString = new byte[] { 0xCA, 0xFE },
                        Integer = 10
                    }
                },
                new byte[] { 0xA0, 0x07, 0x81, 0x02, 0xCA, 0xFE, 0x82, 0x01, 0x0A });
            yield return new TestCaseData(
                new ChoiceElement
                {
                    SequenceElement = new SequenceElement
                    {
                        Integer = 10
                    }
                },
                new byte[] { 0xA0, 0x03, 0x82, 0x01, 0x0A });
            yield return new TestCaseData(
                new ChoiceElement
                {
                    BooleanElement = true
                },
                new byte[] { 0x81, 0x01, 0xFF });
        }

        private static IEnumerable<TestCaseData> SequenceOfModelData()
        {
            yield return new TestCaseData(
                new List<bool> { true, false },
                new byte[] { 0x30, 0x06, 0x01, 0x01, 0xFF, 0x01, 0x01, 0x00 });
            yield return new TestCaseData(
                new bool[] { true, false },
                new byte[] { 0x30, 0x06, 0x01, 0x01, 0xFF, 0x01, 0x01, 0x00 });
        }

        [TestCaseSource(nameof(ChoiceModelData))]
        [TestCaseSource(nameof(SequenceOfModelData))]
        public void ShouldSerializeModel(object element, byte[] expectedData)
        {
            // Act
            var data = AsnSerializer.Serialize(element, AsnEncodingRules.DER);

            // Assert
            Assert.That(data, Is.EqualTo(expectedData));
        }

        [TestCaseSource(nameof(ChoiceModelData))]
        public void ShouldDeserializeToChoiceModel(ChoiceElement expectedElement, byte[] data)
        {
            // Act
            var element = AsnSerializer.Deserialize<ChoiceElement>(data, AsnEncodingRules.DER);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(element.SequenceElement?.Integer, Is.EqualTo(expectedElement.SequenceElement?.Integer));
                Assert.That(element.SequenceElement?.OctetString, Is.EqualTo(expectedElement.SequenceElement?.OctetString));
                Assert.That(element.BooleanElement, Is.EqualTo(expectedElement.BooleanElement));
            });
        }

        [TestCaseSource(nameof(SequenceOfModelData))]
        public void ShouldDeserializeToSequenceOfModel(IEnumerable<bool> expectedElement, byte[] data)
        {
            // Act
            var element = AsnSerializer.Deserialize(data, expectedElement.GetType(), AsnEncodingRules.DER);

            // Assert
            Assert.That(element, Is.EqualTo(expectedElement));
        }



        [Test]
        public void ShouldThrowWhenDeserializing()
        {
            // Arrange
            var data = new byte[] { 0xA0, 0x03, 0x82, 0x01, 0x0A, 0x81, 0x01, 0x00 };

            // Act & Assert
            Assert.Throws<AsnContentException>(() => AsnSerializer.Deserialize<ChoiceElement>(data, AsnEncodingRules.DER));
        }

        [Test]
        public void ShouldThrowWhenSerializing()
        {
            // Arrange
            var model = new ChoiceElement()
            {
                SequenceElement = new SequenceElement() { Integer = 6 },
                BooleanElement = true
            };

            // Act & Assert
            Assert.Throws<AsnConversionException>(() => AsnSerializer.Serialize(model, AsnEncodingRules.DER));
        }


        [AsnSequence]
        public class UniversalSequence
        {
            public BigInteger Integer { get; set; }
        }

        public static IEnumerable<TestCaseData> UniversalTagModelData
        {
            get
            {

                yield return new TestCaseData(
                    new UniversalSequence { Integer = 10 },
                    new byte[] { 0x30, 0x03, 0x02, 0x01, 0x0A });
            }
        }

        [TestCaseSource(nameof(UniversalTagModelData))]
        public void ShouldSerializeUniversalSequence(UniversalSequence model, byte[] data)
        {
            // Act
            var serialized = AsnSerializer.Serialize(model, AsnEncodingRules.DER);

            // Assert
            Assert.That(serialized, Is.EqualTo(data));
        }

        [TestCaseSource(nameof(UniversalTagModelData))]
        public void ShouldDeserializeUniversalSequence(UniversalSequence model, byte[] data)
        {
            // Act
            var deserialized = AsnSerializer.Deserialize<UniversalSequence>(data, AsnEncodingRules.DER);

            // Assert
            Assert.That(deserialized.Integer, Is.EqualTo(model.Integer));
        }
    }
}
