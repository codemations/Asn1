using Codemations.Asn1.Types;
using NUnit.Framework;
using System.Collections.Generic;

namespace Codemations.Asn1.Tests
{
    internal partial class AsnSerializerTests
    {
        private static IEnumerable<TestCaseData> GetStringTestCases()
        {
            yield return new TestCaseData(@"Arek", new byte[] { 0x0C, 0x04, 0x41, 0x72, 0x65, 0x6B });
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
            yield return new TestCaseData((AsnNumericString)@"1234", new byte[] { 0x12, 0x04, 0x31, 0x32, 0x33, 0x34 });
            yield return new TestCaseData((AsnNumericString?)@"1234", new byte[] { 0x12, 0x04, 0x31, 0x32, 0x33, 0x34 });
        }

        [TestCaseSource(nameof(GetStringTestCases))]
        public void Serialize_StringData_ShouldReturnExpectedEncodedValue(object value, byte[] expectedEncodedValue)
        {
            // Act
            var encodedValue = AsnSerializer.SerializeBer(value);

            // Assert
            Assert.That(encodedValue, Is.EqualTo(expectedEncodedValue));
        }

        [TestCaseSource(nameof(GetStringTestCases))]
        public void Deserialize_StringData_ShouldReturnExpectedValue(object expectedValue, byte[] encodedValue)
        {
            // Act
            var value = AsnSerializer.DeserializeBer(encodedValue, expectedValue.GetType());

            // Assert
            Assert.That(value, Is.EqualTo(expectedValue));
        }
    }
}
