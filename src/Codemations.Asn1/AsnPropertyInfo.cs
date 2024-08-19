using System;
using System.Formats.Asn1;
using System.Reflection;


namespace Codemations.Asn1;

public class AsnPropertyInfo
{
    public Asn1Tag? Tag { get; }
    public bool IsOptional { get; }
    public Type Type => _propertyInfo.PropertyType;
    private readonly PropertyInfo _propertyInfo;

    public AsnPropertyInfo(PropertyInfo propertyInfo)
    {
        _propertyInfo = propertyInfo;

        if (propertyInfo.GetCustomAttribute<AsnElementAttribute>() is AsnElementAttribute asnElementAttribute)
        {
            Tag = asnElementAttribute.Tag;
            IsOptional = asnElementAttribute.Optional;
        }
    }

    public static implicit operator AsnPropertyInfo(PropertyInfo propertyInfo) => new(propertyInfo);

    public AsnConverter? GetCustomConverter()
    {
        if (_propertyInfo.GetCustomAttribute<AsnConverterAttribute>() is AsnConverterAttribute asnConverterAttribute)
        {
            return asnConverterAttribute.ConverterType?.CreateInstance<AsnConverter>();
        }
        return null;
    }

    public object? GetValue(object parentObj)
    {
        return _propertyInfo.GetValue(parentObj);
    }

    public void SetValue(object parentObj, object? value)
    {
        _propertyInfo.SetValue(parentObj, value);
    }
}
