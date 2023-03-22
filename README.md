[![Nuget](https://img.shields.io/nuget/v/Codemations.Asn1)](https://www.nuget.org/packages/Codemations.Asn1)
[![Build Status](https://dev.azure.com/aprochwicz/Codemations.Asn1/_apis/build/status/codemations.Asn1?branchName=main)](https://dev.azure.com/aprochwicz/Codemations.Asn1/_build/latest?definitionId=3&branchName=main)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=codemations_Asn1&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=codemations_Asn1)

# Serialization/Deserialization with Codemations.Asn1
## Example class [^1]:
```
[AsnSequence]
public class FooQuestion
{
    [AsnElement]
    public BigInteger TrackingNumber { get; set; }
    [AsnElement]
    public string Question { get; set; }
}
```

## Serialization:
```
var myQuestion = new FooQuestion
{
    TrackingNumber = 5,
    Question = "Anybody there?"
};
AsnConvert.Serialize(myQuestion, AsnEncodingRules.DER);
```

Output (hex):
```
30, 13,
    02, 01, 05,
    16, 0e, 41, 6e, 79, 62, 6f, 64, 79, 20, 74, 68, 65, 72, 65, 3f
```
## Deserialization:
```
var encodedData = new byte[] {
    0x30, 0x13, 0x02, 0x01, 0x05, 0x16, 0x0e, 0x41, 0x6e, 0x79, 0x62, 0x6f, 0x64, 0x79, 0x20, 0x74, 0x68, 0x65, 0x72, 0x65, 0x3f
};

AsnConvert.Deserialize<FooQuestion>(encodedData, AsnEncodingRules.DER);
```

[^1]: https://en.wikipedia.org/wiki/ASN.1
