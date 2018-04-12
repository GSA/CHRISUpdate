using CsvHelper.TypeConversion;
using System;
using CsvHelper;
using CsvHelper.Configuration;
using AutoMapper;

namespace HRUpdate.Mapping
{
    sealed class SSNConverter : ByteConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            Utilities.Helpers helper = new Utilities.Helpers();

            return helper.HashSSN(text);            
        }
    }

    sealed class SSNConverter2 : ByteArrayConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            Utilities.Helpers helper = new Utilities.Helpers();

            return helper.HashSSN(text);
        }
    }

    sealed class FEROConverter: BooleanConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (text.Contains("N"))
                return false;

            return false;
        }
    }

    sealed class LEOConverter: BooleanConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (text.Contains("0"))
                return false;

            return false;
        }
    }
}