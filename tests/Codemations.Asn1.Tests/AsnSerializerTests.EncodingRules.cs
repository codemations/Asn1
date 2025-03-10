﻿using NUnit.Framework;
using System.Collections.Generic;

namespace Codemations.Asn1.Tests;

internal partial class AsnSerializerTests
{
    private static IEnumerable<TestCaseData> GetEncodingRulesData()
    {
        yield return new TestCaseData(123, 
            new byte[] { 0x02, 0x01, 0x7B });
    }

    [TestCaseSource(nameof(GetEncodingRulesData))]
    public void ShouldSerializeBer(object value, byte[] expectedEncodedValue)
    {
        // Act
        var encodedValue = AsnSerializer.SerializeBer(value);

        // Assert
        Assert.That(encodedValue, Is.EqualTo(expectedEncodedValue));
    }

    [TestCaseSource(nameof(GetEncodingRulesData))]
    public void ShouldSerializeBerToSpan(object value, byte[] expectedEncodedValue)
    {
        // Arrange
        var encodedValue = new byte[expectedEncodedValue.Length];

        // Act
        var encodedValueLength = AsnSerializer.SerializeBer(value, encodedValue);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(encodedValueLength, Is.EqualTo(expectedEncodedValue.Length));
            Assert.That(encodedValue, Is.EqualTo(expectedEncodedValue));
        });
    }

    [TestCaseSource(nameof(GetEncodingRulesData))]
    public void ShouldDeserializeBer(object expectedValue, byte[] encodedValue)
    {
        // Act
        var value = AsnSerializer.DeserializeBer(encodedValue, typeof(int));

        // Assert
        Assert.That(value, Is.EqualTo(expectedValue));
    }

    [TestCaseSource(nameof(GetEncodingRulesData))]
    public void ShouldDeserializeBerGeneric(object expectedValue, byte[] encodedValue)
    {
        // Act
        var value = AsnSerializer.DeserializeBer<int>(encodedValue);

        // Assert
        Assert.That(value, Is.EqualTo(expectedValue));
    }

    [TestCaseSource(nameof(GetEncodingRulesData))]
    public void ShouldSerializeCer(object value, byte[] expectedEncodedValue)
    {
        // Act
        var encodedValue = AsnSerializer.SerializeCer(value);

        // Assert
        Assert.That(encodedValue, Is.EqualTo(expectedEncodedValue));
    }

    [TestCaseSource(nameof(GetEncodingRulesData))]
    public void ShouldSerializeCerToSpan(object value, byte[] expectedEncodedValue)
    {
        // Arrange
        var encodedValue = new byte[expectedEncodedValue.Length];

        // Act
        var encodedValueLength = AsnSerializer.SerializeCer(value, encodedValue);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(encodedValueLength, Is.EqualTo(expectedEncodedValue.Length));
            Assert.That(encodedValue, Is.EqualTo(expectedEncodedValue));
        });
    }

    [TestCaseSource(nameof(GetEncodingRulesData))]
    public void ShouldDeserializeCer(object expectedValue, byte[] encodedValue)
    {
        // Act
        var value = AsnSerializer.DeserializeCer(encodedValue, typeof(int));

        // Assert
        Assert.That(value, Is.EqualTo(expectedValue));
    }

    [TestCaseSource(nameof(GetEncodingRulesData))]
    public void ShouldDeserializeCerGeneric(object expectedValue, byte[] encodedValue)
    {
        //Act
        var value = AsnSerializer.DeserializeCer<int>(encodedValue);

        // Assert
        Assert.That(value, Is.EqualTo(expectedValue));
    }

    [TestCaseSource(nameof(GetEncodingRulesData))]
    public void ShouldSerializeDer(object value, byte[] expectedEncodedValue)
    {
        //Act
        var encodedValue = AsnSerializer.SerializeDer(value);

        // Assert
        Assert.That(encodedValue, Is.EqualTo(expectedEncodedValue));
    }

    [TestCaseSource(nameof(GetEncodingRulesData))]
    public void ShouldSerializeDerToSpan(object value, byte[] expectedEncodedValue)
    {
        // Arrange
        var encodedValue = new byte[expectedEncodedValue.Length];

        // Act
        var encodedValueLength = AsnSerializer.SerializeDer(value, encodedValue);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(encodedValueLength, Is.EqualTo(expectedEncodedValue.Length));
            Assert.That(encodedValue, Is.EqualTo(expectedEncodedValue));
        });
    }

    [TestCaseSource(nameof(GetEncodingRulesData))]
    public void ShouldDeserializeDer(object expectedValue, byte[] encodedValue)
    {
        // Act
        var value = AsnSerializer.DeserializeDer(encodedValue, typeof(int));

        // Assert
        Assert.That(value, Is.EqualTo(expectedValue));
    }

    [TestCaseSource(nameof(GetEncodingRulesData))]
    public void ShouldDeserializeDerGeneric(object expectedValue, byte[] encodedValue)
    {
        // Act
        var value = AsnSerializer.DeserializeDer<int>(encodedValue);

        // Arrange
        Assert.That(value, Is.EqualTo(expectedValue));
    }
}
