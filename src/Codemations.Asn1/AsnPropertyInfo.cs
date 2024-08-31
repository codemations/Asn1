using Codemations.Asn1.Attributes;
using Codemations.Asn1.Extensions;
using System;
using System.Formats.Asn1;
using System.Reflection;

namespace Codemations.Asn1;

public class AsnPropertyInfo
{
    public Asn1Tag? Tag { get; }
    public bool IsRequired { get; }
    public Type PropertyType => _propertyInfo.PropertyType;
    public Type? ConverterType { get; }
    private readonly PropertyInfo _propertyInfo;

    public AsnPropertyInfo(PropertyInfo propertyInfo)
    {
        _propertyInfo = propertyInfo;
        var attributes = propertyInfo.GetCustomAttributes(typeof(AsnAttribute), true);

        Tag = GetAttribute<AsnTagAttribute>(attributes)?.Tag;
        IsRequired = GetAttribute<AsnOptionalAttribute>(attributes) is null;
        ConverterType = GetAttribute<AsnConverterAttribute>(attributes)?.ConverterType;
    }

    public static implicit operator AsnPropertyInfo(PropertyInfo propertyInfo) => new(propertyInfo);

    public AsnConverter? GetAsnConverter()
    {
        return ConverterType?.CreateInstance<AsnConverter>();
    }

    public object? GetValue(object parentObj)
    {
        return _propertyInfo.GetValue(parentObj);
    }

    public void SetValue(object parentObj, object? value)
    {
        _propertyInfo.SetValue(parentObj, value);
    }

    private static T? GetAttribute<T>(object[] attributes) where T: Attribute
    {
        return Array.Find(attributes, attribute => attribute is T) as T;
    }
}
