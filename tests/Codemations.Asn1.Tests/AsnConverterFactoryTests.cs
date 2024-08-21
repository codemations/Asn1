using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Numerics;
using Codemations.Asn1.Extensions;
using NUnit.Framework;

namespace Codemations.Asn1.Tests;

public class AsnConverterFactoryTests
{
    public enum TestEnum
    {
        Success = 0x7F,
        Failure = 0x80
    }

    public static IEnumerable<TestCaseData> Data
    {
        get
        {
            yield return new TestCaseData( false, (byte)0x80, new byte[] { 0x80, 0x01, 000 } );
            yield return new TestCaseData( true, (byte)0x81, new byte[] { 0x81, 0x01, 0xFF } );
            yield return new TestCaseData((BigInteger)10, (byte)0x82, new byte[] { 0x82, 0x01, 0x0A } );
            yield return new TestCaseData( new byte[] { 0xCA, 0xFE }, (byte)0x83, new byte[] { 0x83, 0x02, 0xCA, 0xFE } );
            yield return new TestCaseData(new byte[] { 0xCA, 0xFE }, (byte)0x84, new byte[] { 0x84, 0x02, 0xCA, 0xFE });
            yield return new TestCaseData(TestEnum.Success, (byte)0x85, new byte[] { 0x85, 0x01, 0x7F });
            yield return new TestCaseData(TestEnum.Failure, (byte)0x86, new byte[] { 0x86, 0x02, 0x00, 0x80 });
            yield return new TestCaseData(@"Arek", (byte)0x87, new byte[] { 0x87, 0x04, 0x41, 0x72, 0x65, 0x6B });
        }
    }

    [AsnSequence]
    public class TestSequenceOfElement
    {
        [AsnElement(0x80)] public bool Boolean { get; set; }
        [AsnElement(0x81)] public BigInteger Integer { get; set; }
        [AsnElement(0x82, Optional = true)] public TestEnum? Enum { get; set; }
    }

    public static IEnumerable<TestCaseData> SequenceOfData
    {
        get
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
    }

    [TestCaseSource(nameof(Data))]
    [TestCaseSource(nameof(SequenceOfData))]
    public void ShouldWriteValue(object value, byte tag, byte[] expectedEncodedValue)
    {
        // Arrange
        var writer = new AsnWriter(AsnEncodingRules.BER);
        var serializer = new AsnSerializer();
        var converter = AsnConvertersList.CreateDefault().Get(value.GetType());

        // Act
        converter.Write(writer, tag.ToAsn1Tag(), value, serializer);
        var actualEncodedValue = writer.Encode();

        // Assert
        Assert.That(actualEncodedValue, Is.EqualTo(expectedEncodedValue));
    }

    [TestCaseSource(nameof(Data))]
    public void ShouldReadValue(object expectedValue, byte tag, byte[] encodedValue)
    {
        // Arrange
        var reader = new AsnReader(encodedValue, AsnEncodingRules.BER);
        var serializer = new AsnSerializer();
        var converter = AsnConvertersList.CreateDefault().Get(expectedValue.GetType());

        // Act
        var actualValue = converter.Read(reader, tag.ToAsn1Tag(), expectedValue.GetType(), serializer);

        // Assert
        Assert.That(actualValue, Is.EqualTo(expectedValue));
    }

    [TestCaseSource(nameof(SequenceOfData))]
    public void ShouldReadSequenceOf(List<TestSequenceOfElement> expectedValue, byte tag, byte[] encodedValue)
    {
        // Arrange
        var reader = new AsnReader(encodedValue, AsnEncodingRules.BER);
        var serializer = new AsnSerializer();
        var converter = AsnConvertersList.CreateDefault().Get(expectedValue.GetType());

        // Act
        var actualValue = (List<TestSequenceOfElement>)converter.Read(reader, tag.ToAsn1Tag(), expectedValue.GetType(), serializer);

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