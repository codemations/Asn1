﻿using Codemations.Asn1.Converters;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;

namespace Codemations.Asn1
{

    public class AsnSerializer
    {
        public AsnEncodingRules EncodingRules { get; }
        public AsnReaderOptions ReaderOptions { get; }
        private readonly AsnConvertersList _converterList;
        public IList<IAsnConverter> Converters => _converterList;

        public AsnSerializer(AsnEncodingRules encodingRules, AsnReaderOptions readerOptions = default)
        {
            EncodingRules = encodingRules;
            ReaderOptions = readerOptions;
            _converterList = AsnConvertersList.CreateDefault();
        }

        public object Deserialize(ReadOnlyMemory<byte> data, Type type)
        {
            var reader = new AsnReader(data, this.EncodingRules, this.ReaderOptions);
            var deserialized = Deserialize(reader, type);

            if (reader.HasData)
            {
                throw new AsnConversionException("Not read data left.");
            }

            return deserialized;
        }

        public object Deserialize(AsnReader reader, Type propertyType)
        {
            return Deserialize(reader, tag: null, propertyType, customConverter: null);
        }

        public object Deserialize(AsnReader reader, AsnPropertyInfo asnPropertyInfo)
        {
            return Deserialize(reader, asnPropertyInfo.Tag, asnPropertyInfo.Type, asnPropertyInfo.GetCustomConverter());
        }

        public object Deserialize(AsnReader reader, Asn1Tag? tag, Type propertyType, IAsnConverter? customConverter)
        {
            var converter = customConverter ?? _converterList.Get(propertyType);
            return converter.Read(reader, tag, propertyType, this);
        }

        public byte[] Serialize(object value)
        {
            var writer = new AsnWriter(this.EncodingRules);
            Serialize(writer, value);
            return writer.Encode();
        }

        public void Serialize(AsnWriter writer, object value)
        {
            Serialize(writer, null, value, null);
        }

        public void Serialize(AsnWriter writer, AsnPropertyInfo asnPropertyInfo, object value)
        {
            Serialize(writer, asnPropertyInfo.Tag, value, asnPropertyInfo.GetCustomConverter());
        }

        public void Serialize(AsnWriter writer, Asn1Tag? tag, object value, IAsnConverter? customConverter)
        {
            var converter = customConverter ?? _converterList.Get(value.GetType());
            converter.Write(writer, tag, value, this);
        }
    }
}