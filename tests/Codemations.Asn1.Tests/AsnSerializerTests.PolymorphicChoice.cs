using Codemations.Asn1.Attributes;
using NUnit.Framework;
using System.Collections.Generic;
using System.Numerics;

namespace Codemations.Asn1.Tests;

internal partial class AsnSerializerTests
{
    [AsnPolymorphicChoice(typeof(GetRequestPdu), 0)]
    [AsnPolymorphicChoice(typeof(GetNextRequestPdu), 1)]
    public abstract record Pdu
    {
        public BigInteger RequestId { get; set; }
    }

    public record GetRequestPdu : Pdu
    {

    }

    public record GetNextRequestPdu : Pdu
    {

    }

    private static IEnumerable<TestCaseData> GetPolymorphicChoiceTestCases()
    {
        yield return new TestCaseData(new GetRequestPdu(), new byte[] { 0xA0, 0x03, 0x02, 0x01, 0x00 });
        yield return new TestCaseData(new GetRequestPdu() { RequestId = 1}, new byte[] { 0xA0, 0x03, 0x02, 0x01, 0x01 });
        yield return new TestCaseData(new GetNextRequestPdu(), new byte[] { 0xA1, 0x03, 0x02, 0x01, 0x00 });
        yield return new TestCaseData(new GetNextRequestPdu() { RequestId = 1 }, new byte[] { 0xA1, 0x03, 0x02, 0x01, 0x01 });
    }

    [TestCaseSource(nameof(GetPolymorphicChoiceTestCases))]
    public void Serialize_PolymorphicChoice_ShouldReturnExpectedEncodedData(Pdu value, byte[] expectedEncodedValue)
    {
        // Act
        var encodedPdu = AsnSerializer.SerializeBer(value);

        // Assert
        Assert.That(encodedPdu, Is.EqualTo(expectedEncodedValue));
    }

    [TestCaseSource(nameof(GetPolymorphicChoiceTestCases))]
    public void Deserialize_PolymorphicChoice_ShouldReturnExpectedValue(Pdu expectedValue, byte[] encodedValue)
    {
        // Act
        var value = AsnSerializer.DeserializeBer(encodedValue, expectedValue.GetType());

        // Assert
        Assert.That(value, Is.EqualTo(expectedValue));
    }
}
