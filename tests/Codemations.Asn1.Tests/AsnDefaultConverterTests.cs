using System.Collections.Generic;
using System.Formats.Asn1;
using System.Numerics;
using Xunit;

namespace Codemations.Asn1.Tests;

public class AsnDefaultConverterTests
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
            yield return new object[] { false, 0x80, new byte[] { 0x80, 0x01, 000 } };
            yield return new object[] { true, 0x81, new byte[] { 0x81, 0x01, 0xFF } };
            yield return new object[] { (BigInteger)10, 0x82, new byte[] { 0x82, 0x01, 0x0A } };
            yield return new object[] { new byte[] { 0xCA, 0xFE }, 0x83, new byte[] { 0x83, 0x02, 0xCA, 0xFE } };
            yield return new object[] { new byte[] { 0xCA, 0xFE }, 0x84, new byte[] { 0x84, 0x02, 0xCA, 0xFE } };
            yield return new object[] { TestEnum.Success, 0x85, new byte[] { 0x85, 0x01, 0x7F } };
            yield return new object[] { TestEnum.Failure, 0x86, new byte[] { 0x86, 0x02, 0x00, 0x80 } };
            yield return new object[] { "1.2.840.113583", 0x87, new byte[] { 0x87, 0x06, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x2F } };
        }
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void ShouldWriteValue(object value, byte tag, byte[] expectedEncodedValue)
    {
        // Arrange
        var writer = new AsnWriter(AsnEncodingRules.DER);
        var converter = new AsnDefaultConverter();

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
        var converter = new AsnDefaultConverter();

        // Act
        
        var actualValue = converter.Read(reader, tag.ToAsn1Tag(), expectedValue.GetType());

        // Assert
        Assert.Equal(expectedValue, actualValue);
    }
}