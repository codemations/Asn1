using System.Linq;
using Xunit;

namespace Codemations.Asn1.Tests
{
    public partial class AsnConvertTests
    {
        [AsnSequence]
        public class ChoiceSequence
        {
            public ChoiceElement? Choice1 { get; set; }

            public ChoiceElement? Choice2 { get; set; }
        }


        [Fact]
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

            Assert.Equal(choiceSequenceEncoded, actualEncodedValue);
        }

        [Fact]
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

            Assert.Equal(choiceSequence.Choice1.SequenceElement.Integer, actualChoiceSequence.Choice1?.SequenceElement?.Integer);
            Assert.Equal(choiceSequence.Choice2.BooleanElement, actualChoiceSequence.Choice2?.BooleanElement);
        }
    }
}
