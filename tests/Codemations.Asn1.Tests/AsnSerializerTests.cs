using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Codemations.Asn1.Attributes;
using Codemations.Asn1.Extensions;
using NUnit.Framework;

namespace Codemations.Asn1.Tests;

internal partial class AsnSerializerTests
{
    public enum TestEnum
    {
        Success = 0x7F,
        Failure = 0x80
    }

    public class TestSequenceOfElement
    {
        [AsnTag(0x80)] 
        public bool Boolean { get; set; }

        [AsnTag(0x81)]
        public BigInteger Integer { get; set; }
        
        [AsnTag(0x82)]
        [AsnOptional]
        public TestEnum? Enum { get; set; }
    }

    private static IEnumerable<TestCaseData> GetSequenceOfTestCases()
    {
        yield return new TestCaseData(
            new List<TestSequenceOfElement>
            {
                new() {Boolean = false, Integer = 10},
                new() {Boolean = true, Integer = 20}
            },
            (byte)0xA8,
            new byte[]
            {
                0xA8, 0x10,
                    0x30, 0x06,
                        0x80, 0x01, 0x00,
                        0x81, 0x01, 0x0A,
                    0x30, 0x06,
                        0x80, 0x01, 0xFF,
                        0x81, 0x01, 0x14
            });
        yield return new TestCaseData(
            new List<TestSequenceOfElement>
            {
                new()
                {
                    Boolean = false,
                    Integer = 10,
                    Enum = TestEnum.Success
                }
            },
            (byte)0xA8,
            new byte[]
            {
                0xA8, 0x0B,
                0x30, 0x09,
                0x80, 0x01, 0x00,
                0x81, 0x01, 0x0A,
                0x82, 0x01, 0x7F,
            });
    }

    [TestCaseSource(nameof(GetSequenceOfTestCases))]
    public void Serialize_ShouldReturnExpectedEncodedValue(object value, byte tag, byte[] expectedEncodedValue)
    {
        // Arrange
        var asnTag = tag.ToAsn1Tag();

        // Act
        var actualEncodedValue = AsnSerializer.SerializeBer(value, asnTag);

        // Assert
        Assert.That(actualEncodedValue, Is.EqualTo(expectedEncodedValue));
    }

    [TestCaseSource(nameof(GetSequenceOfTestCases))]
    public void ShouldReadSequenceOf(List<TestSequenceOfElement> expectedValue, byte tag, byte[] encodedValue)
    {
        // Arrange
        var asnTag = tag.ToAsn1Tag();

        // Act
        var actualValue = (List<TestSequenceOfElement>)AsnSerializer.DeserializeBer(encodedValue, expectedValue.GetType(), asnTag);

        // Assert
        Assert.Multiple(() =>
        {
            foreach (var (expectedItem, actualItem) in expectedValue.Zip(actualValue, (e, a) => (e, a)))
            {
                Assert.That(actualItem.Boolean, Is.EqualTo(expectedItem.Boolean));
                Assert.That(actualItem.Integer, Is.EqualTo(expectedItem.Integer));
                Assert.That(actualItem.Enum, Is.EqualTo(expectedItem.Enum));
            }
        });
    }
}