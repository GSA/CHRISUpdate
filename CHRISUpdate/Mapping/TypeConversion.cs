using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using HRUpdate.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HRUpdate.Mapping
{
    internal sealed class SocialSecurityNumberConverter : ByteConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            Utilities.Helpers helper = new Utilities.Helpers();

            return helper.HashSSN(text);
        }
    }

    internal sealed class PositionTeleworkEligibilityConverter : BooleanConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            switch (text)
            {
                case "Y":
                    return true;

                case "N":
                    return false;

                default:
                    return false;
            }
        }
    }

    internal sealed class FederalEmergencyResponseOfficialConverter : BooleanConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (text.Contains("N"))
                return false;

            return false;
        }
    }

    internal sealed class LawEnforcementOfficerConverter : BooleanConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            switch (text)
            {
                case "1":
                    return true;

                case "0":
                    return false;

                default:
                    return false;
            }
        }
    }

    internal sealed class InvistigationResultConverter : BooleanConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            switch (text)
            {
                case "1":
                    return true;

                case "0":
                    return false;

                default:
                    return false;
            }
        }
    }

    internal sealed class DateConverter : DateTimeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            DateTime dt;

            if (DateTime.TryParse(text, out dt))
                return dt;
            else
                return null;
        }
    }

    internal sealed class RegionConverter : StringConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {  
            if (text.Equals("11"))
                return "NCR";

            if (text.Equals("0"))
                return "10";

            return text.PadLeft(2, '0');
        }
    }

    internal sealed class InvestigationConverter : StringConverter
    {
        private readonly List<InvestigationLookup> investigationLookup;

        public InvestigationConverter(List<InvestigationLookup> investigationLookup)
        {
            this.investigationLookup = investigationLookup;
        }

        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            string investigation = string.Empty;

            investigation = investigationLookup.Where(w => w.Code == text).Select(s => s.Tier).SingleOrDefault();

            if (string.IsNullOrEmpty(investigation))
                return text;

            return investigation;
        }
    }

    internal sealed class StateCodeConverter : StringConverter
    {
        private readonly List<StateLookup> stateLookup;

        public StateCodeConverter(List<StateLookup> stateLookup)
        {
            this.stateLookup = stateLookup;
        }

        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            string state = string.Empty;

            state = stateLookup.Where(w => w.Code == text).Select(s => s.Code).SingleOrDefault();

            if (string.IsNullOrEmpty(state))
                return text;

            return state;
        }
    }

    internal sealed class CountryCodeConverter : StringConverter
    {
        private readonly List<CountryLookup> countryLookup;

        public CountryCodeConverter(List<CountryLookup> countryLookup)
        {
            this.countryLookup = countryLookup;
        }

        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            string country = string.Empty;

            country = countryLookup.Where(w => w.Code == text).Select(s => s.Code).SingleOrDefault();

            if (string.IsNullOrEmpty(country))
                return text;

            return country;
        }
    }
}