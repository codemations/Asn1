using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Codemations.Asn1.Tests
{
    public class AsnConvertEncodingRulesTests
    {
        private static IEnumerable<TestCaseData> GetEncodingRulesData()
        {
            yield return new TestCaseData("dummy-text", 
                new byte[] { 0x0C, 0x0A, 0x64, 0x75, 0x6D, 0x6D, 0x79, 0x2D, 0x74, 0x65, 0x78, 0x74 });
        }

        [TestCaseSource(nameof(GetEncodingRulesData))]
        public void ShouldSerializeBer(object value, byte[] expectedEncodedValue)
        {
            //Act
            var encodedValue = AsnSerializer.SerializeBer(value);

            // Arrange
            Assert.That(encodedValue, Is.EqualTo(expectedEncodedValue));
        }

        [TestCaseSource(nameof(GetEncodingRulesData))]
        public void ShouldDeserializeBer(object expectedValue, byte[] encodedValue)
        {
            //Act
            var value = AsnSerializer.DeserializeBer(encodedValue, typeof(string));

            // Arrange
            Assert.That(value, Is.EqualTo(expectedValue));
        }

        [TestCaseSource(nameof(GetEncodingRulesData))]
        public void ShouldDeserializeBerGeneric(object expectedValue, byte[] encodedValue)
        {
            //Act
            var value = AsnSerializer.DeserializeBer<string>(encodedValue);

            // Arrange
            Assert.That(value, Is.EqualTo(expectedValue));
        }

        [TestCaseSource(nameof(GetEncodingRulesData))]
        public void ShouldSerializeCer(object value, byte[] expectedEncodedValue)
        {
            //Act
            var encodedValue = AsnSerializer.SerializeCer(value);

            // Arrange
            Assert.That(encodedValue, Is.EqualTo(expectedEncodedValue));
        }

        [TestCaseSource(nameof(GetEncodingRulesData))]
        public void ShouldDeserializeCer(object expectedValue, byte[] encodedValue)
        {
            //Act
            var value = AsnSerializer.DeserializeCer(encodedValue, typeof(string));

            // Arrange
            Assert.That(value, Is.EqualTo(expectedValue));
        }

        [TestCaseSource(nameof(GetEncodingRulesData))]
        public void ShouldDeserializeCerGeneric(object expectedValue, byte[] encodedValue)
        {
            //Act
            var value = AsnSerializer.DeserializeCer<string>(encodedValue);

            // Arrange
            Assert.That(value, Is.EqualTo(expectedValue));
        }

        [TestCaseSource(nameof(GetEncodingRulesData))]
        public void ShouldSerializeDer(object value, byte[] expectedEncodedValue)
        {
            //Act
            var encodedValue = AsnSerializer.SerializeDer(value);

            // Arrange
            Assert.That(encodedValue, Is.EqualTo(expectedEncodedValue));
        }

        [TestCaseSource(nameof(GetEncodingRulesData))]
        public void ShouldDeserializeDer(object expectedValue, byte[] encodedValue)
        {
            //Act
            var value = AsnSerializer.DeserializeDer(encodedValue, typeof(string));

            // Arrange
            Assert.That(value, Is.EqualTo(expectedValue));
        }

        [TestCaseSource(nameof(GetEncodingRulesData))]
        public void ShouldDeserializeDerGeneric(object expectedValue, byte[] encodedValue)
        {
            //Act
            var value = AsnSerializer.DeserializeDer<string>(encodedValue);

            // Arrange
            Assert.That(value, Is.EqualTo(expectedValue));
        }
    }
}
