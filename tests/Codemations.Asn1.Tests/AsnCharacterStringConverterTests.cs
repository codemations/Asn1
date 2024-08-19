using Codemations.Asn1.Converters;

using Codemations.Asn1.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;

namespace Codemations.Asn1.Tests
{
    internal class AsnCharacterStringConverterTests
    {
        private static IEnumerable<TestCaseData> GetStringTestData()
        {
            yield return new TestCaseData(@"Arek", new byte[] { 0x16, 0x04, 0x41, 0x72, 0x65, 0x6B });
            yield return new TestCaseData((AsnUtf8String)@"Arek", new byte[] { 0x0C, 0x04, 0x41, 0x72, 0x65, 0x6B });
            yield return new TestCaseData((AsnUtf8String?)@"Arek", new byte[] { 0x0C, 0x04, 0x41, 0x72, 0x65, 0x6B });
            yield return new TestCaseData((AsnIA5String)@"Arek", new byte[] { 0x16, 0x04, 0x41, 0x72, 0x65, 0x6B });
            yield return new TestCaseData((AsnIA5String?)@"Arek", new byte[] { 0x16, 0x04, 0x41, 0x72, 0x65, 0x6B });
            yield return new TestCaseData((AsnVisibleString)@"Arek", new byte[] { 0x1A, 0x04, 0x41, 0x72, 0x65, 0x6B });
            yield return new TestCaseData((AsnVisibleString?)@"Arek", new byte[] { 0x1A, 0x04, 0x41, 0x72, 0x65, 0x6B });
            yield return new TestCaseData((AsnPrintableString)@"Arek", new byte[] { 0x13, 0x04, 0x41, 0x72, 0x65, 0x6B });
            yield return new TestCaseData((AsnPrintableString?)@"Arek", new byte[] { 0x13, 0x04, 0x41, 0x72, 0x65, 0x6B });
            yield return new TestCaseData((AsnT61String)@"Arek", new byte[] { 0x14, 0x04, 0x41, 0x72, 0x65, 0x6B });
            yield return new TestCaseData((AsnT61String?)@"Arek", new byte[] { 0x14, 0x04, 0x41, 0x72, 0x65, 0x6B });
            yield return new TestCaseData((AsnBmpString)@"Arek", new byte[] { 0x1E, 0x08, 0x00, 0x41, 0x00, 0x72, 0x00, 0x65, 0x00, 0x6B });
            yield return new TestCaseData((AsnBmpString?)@"Arek", new byte[] { 0x1E, 0x08, 0x00, 0x41, 0x00, 0x72, 0x00, 0x65, 0x00, 0x6B });
        }
        

        [TestCaseSource(nameof(GetStringTestData))]
        public void ShouldSerializeString(object value, byte[] expectedEncodedValue)
        {
            // Arrange
            var serializer = new AsnSerializer(AsnEncodingRules.BER);

            // Act
            var encodedValue = serializer.Serialize(value);

            // Assert
            Assert.That(encodedValue, Is.EqualTo(expectedEncodedValue));
        }

        [TestCaseSource(nameof(GetStringTestData))]
        public void ShouldDeserializeString(object expectedValue, byte[] encodedValue)
        {
            // Arrange
            var serializer = new AsnSerializer(AsnEncodingRules.BER);

            // Act
            var value = serializer.Deserialize(encodedValue, expectedValue.GetType());

            // Assert
            Assert.That(value, Is.EqualTo(expectedValue));
        }
    }
}
