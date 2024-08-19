using System;
using System.Formats.Asn1;

namespace Codemations.Asn1;

public abstract class AsnConverter
{
    public abstract bool CanConvert(Type type);
    public abstract object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer);
    public abstract void Write(AsnWriter writer, Asn1Tag? tag, object value, AsnSerializer serializer);
}

internal abstract class AsnConverter<T> : AsnConverter where T : notnull
{
    protected abstract T ReadTyped(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer);
    protected abstract void WriteTyped(AsnWriter writer, Asn1Tag? tag, T value, AsnSerializer serializer);

    public override bool CanConvert(Type type)
    {
        return type == typeof(T);
    }

    public override object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        return ReadTyped(reader, tag, type, serializer);
    }

    public override void Write(AsnWriter writer, Asn1Tag? tag, object value, AsnSerializer serializer)
    {
        WriteTyped(writer, tag, (T)value, serializer);
    }
}
