using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Numerics;
using Xunit;

namespace Codemations.Asn1.Tests;

public class AsnConverterFactoryTests
{
    public enum TestEnum
    {
        Success = 0x7F,
        Failure = 0x80
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            yield return new object[] {false, 0x80, new byte[] {0x80, 0x01, 000}};
            yield return new object[] {true, 0x81, new byte[] {0x81, 0x01, 0xFF}};
            yield return new object[] {(BigInteger) 10, 0x82, new byte[] {0x82, 0x01, 0x0A}};
            yield return new object[] {new byte[] {0xCA, 0xFE}, 0x83, new byte[] {0x83, 0x02, 0xCA, 0xFE}};
            yield return new object[] {new byte[] {0xCA, 0xFE}, 0x84, new byte[] {0x84, 0x02, 0xCA, 0xFE}};
            yield return new object[] {TestEnum.Success, 0x85, new byte[] {0x85, 0x01, 0x7F}};
            yield return new object[] {TestEnum.Failure, 0x86, new byte[] {0x86, 0x02, 0x00, 0x80}};
            yield return new object[] {@"Arek", 0x87, new byte[] {0x87, 0x04, 0x41, 0x72, 0x65, 0x6B}};
        }
    }

    [AsnSequence]
    public class TestSequenceOfElement
    {
        [AsnElement(0x80)] public bool Boolean { get; set; }
        [AsnElement(0x81)] public BigInteger Integer { get; set; }
        [AsnElement(0x82, Optional = true)] public TestEnum? Enum { get; set; }
    }

    public static IEnumerable<object[]> SequenceOfData
    {
        get
        {
            yield return new object[]
            {
                new List<TestSequenceOfElement>
                {
                    new() {Boolean = false, Integer = 10},
                    new() {Boolean = true, Integer = 20}
                },
                0xA8,
                new byte[]
                {
                    0xA8, 0x10,
                        0x30, 0x06,
                            0x80, 0x01, 0x00,
                            0x81, 0x01, 0x0A,
                        0x30, 0x06,
                            0x80, 0x01, 0xFF,
                            0x81, 0x01, 0x14
                }
            };
            yield return new object[]
            {
                new List<TestSequenceOfElement>
                {
                    new()
                    {
                        Boolean = false, 
                        Integer = 10,
                        Enum = TestEnum.Success
                    }
                },

                0xA8,
                new byte[]
                {
                    0xA8, 0x0B,
                    0x30, 0x09,
                    0x80, 0x01, 0x00,
                    0x81, 0x01, 0x0A,
                    0x82, 0x01, 0x7F,
                }
            };

        }
    }

    [Theory]
    [MemberData(nameof(Data))]
    [MemberData(nameof(SequenceOfData))]
    public void ShouldWriteValue(object value, byte tag, byte[] expectedEncodedValue)
    {
        // Arrange
        var writer = new AsnWriter(AsnEncodingRules.DER);
        var converter = new AsnConverterFactory().CreateElementConverter(value.GetType());

        // Act
        converter.Write(writer, tag.ToAsn1Tag(), value);
        var actualEncodedValue = writer.Encode();

        // Assert
        Assert.Equal(expectedEncodedValue, actualEncodedValue);
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void ShouldReadValue(object expectedValue, byte tag, byte[] encodedValue)
    {
        // Arrange
        var reader = new AsnReader(encodedValue, AsnEncodingRules.DER);
        var converter = new AsnConverterFactory().CreateElementConverter(expectedValue.GetType());

        // Act
        var actualValue = converter.Read(reader, tag.ToAsn1Tag(), expectedValue.GetType());

        // Assert
        Assert.Equal(expectedValue, actualValue);
    }

    [Theory]
    [MemberData(nameof(SequenceOfData))]
    public void ShouldReadSequenceOf(List<TestSequenceOfElement> expectedValue, byte tag, byte[] encodedValue)
    {
        // Arrange
        var reader = new AsnReader(encodedValue, AsnEncodingRules.DER);
        var converter = new AsnConverterFactory().CreateElementConverter(expectedValue.GetType());

        // Act
        var actualValue = (List<TestSequenceOfElement>)converter.Read(reader, tag.ToAsn1Tag(), expectedValue.GetType());

        // Assert
        foreach (var (expectedItem, actualItem) in expectedValue.Zip(actualValue))
        {
            Assert.Equal(expectedItem.Boolean, actualItem.Boolean);
            Assert.Equal(expectedItem.Integer, actualItem.Integer);
            Assert.Equal(expectedItem.Enum, actualItem.Enum);
        }
    }
}