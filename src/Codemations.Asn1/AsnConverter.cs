using System;
using System.Formats.Asn1;

namespace Codemations.Asn1;

public abstract class AsnConverter
{
    public abstract bool CanConvert(Type type);
    public abstract object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer);
    public abstract void Write(AsnWriter writer, Asn1Tag? tag, object value, AsnSerializer serializer);
}

public abstract class AsnConverter<T> : AsnConverter where T : notnull
{    
    public override bool CanConvert(Type type)
    {
        return type == typeof(T);
    }

    public override void Write(AsnWriter writer, Asn1Tag? tag, object value, AsnSerializer serializer)
    {
        Write(writer, tag, (T)value, serializer);
    }

    public abstract void Write(AsnWriter writer, Asn1Tag? tag, T value, AsnSerializer serializer);
}
