using CsvHelper.TypeConversion;
using System;

namespace CHRISUpdate.Mapping
{
    //changed the poco classes to accept null values nullable dates DateTime?
    sealed class DateConverter : DateTimeConverter
    {
        public override bool CanConvertFrom(Type type)
        {
            return typeof(string) == type;
        }

        public override bool CanConvertTo(Type type)
        {
            return typeof(DateTime) == type;
        }

        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            DateTime date;

            if (DateTime.TryParse(text, out date))
                return date;

            return date;
        }
    }

    sealed class AssignmentConverter : BooleanConverter
    {
        public override bool CanConvertFrom(Type type)
        {
            return typeof(string) == type;
        }

        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            //Look into returning !text.ToLower().Contains("detail")

            return text.ToLower().Contains("detail");

            //if (text.ToLower().Contains("detail"))
            //    return true;

            //return false;
        }
    }

    sealed class SSNConverter : StringConverter
    {
        Utilities.Utilities u = new Utilities.Utilities();

        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            Console.WriteLine(text);
            if (string.IsNullOrEmpty(text))
                return null;

            return u.HashSSN(text);
        }
    }

    sealed class RegionConverter : StringConverter
    {
        public override bool CanConvertFrom(Type type)
        {
            return typeof(string) == type;
        }

        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            if (text.Contains("CO") || text.Contains("NCR"))
                return text;

            return text.PadLeft(2, '0');
        }
    }
}