using System;
using System.Formats.Asn1;
using System.Reflection;


namespace Codemations.Asn1
{
    public class AsnProperty
    {
        public Asn1Tag? Tag { get; }
        public Type PropertyType { get; }
        public bool IsOptional { get; }

        private readonly object? _parentObject;
        private readonly PropertyInfo? _propertyInfo;

        public AsnProperty(Asn1Tag? tag, Type propertyType, bool isOptional)
        {
            Tag = tag;
            PropertyType = propertyType;
            IsOptional = isOptional;
        }

        public AsnProperty(object parentObject, PropertyInfo propertyInfo)
        {
            _parentObject = parentObject;
            _propertyInfo = propertyInfo;
            PropertyType = propertyInfo.PropertyType;
            if (propertyInfo.GetCustomAttribute<AsnElementAttribute>() is AsnElementAttribute asnElementAttribute)
            {
                Tag = asnElementAttribute.Tag;
                IsOptional = asnElementAttribute.Optional;
            }
        }

        public IAsnConverter? GetConverter()
        {
            return _propertyInfo?.GetCustomAttribute<AsnConverterAttribute>()?.CreateInstance();
        }

        public object? GetValue()
        {
            if(_propertyInfo is null || _parentObject is null)
            {
                throw new InvalidOperationException();
            }

            return _propertyInfo.GetValue(_parentObject);
        }

        public void SetValue(object? value)
        {
            if (_propertyInfo is null || _parentObject is null)
            {
                throw new InvalidOperationException();
            }

            _propertyInfo.SetValue(_parentObject, value);
        }
    }


}
