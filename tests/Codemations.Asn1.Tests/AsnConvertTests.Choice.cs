using NUnit.Framework;
using System.Linq;

namespace Codemations.Asn1.Tests
{
    public partial class AsnConvertTests
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

            var actualEncodedValue = AsnConvert.Serialize(choiceSequence, System.Formats.Asn1.AsnEncodingRules.BER);

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

            var actualChoiceSequence = AsnConvert.Deserialize<ChoiceSequence>(choiceSequenceEncoded, System.Formats.Asn1.AsnEncodingRules.BER);

            Assert.Multiple(() =>
            {
                Assert.That(actualChoiceSequence.Choice1?.SequenceElement?.Integer, Is.EqualTo(choiceSequence.Choice1.SequenceElement.Integer));
                Assert.That(actualChoiceSequence.Choice2?.BooleanElement, Is.EqualTo(choiceSequence.Choice2.BooleanElement));
            });
        }
    }
}
