using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using HRUpdate.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
            switch (text.ToLower())
            {
                case "y":
                    return true;

                case "n":
                    return false;

                default:
                    return false;
            }
        }
    }

    /// <summary>
    /// If first letter equals O return A, if W return P, otherwise return first letter
    /// </summary>
    internal sealed class MajorOrgConverter : StringConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            string officeSymbol = string.Empty;

            officeSymbol = Regex.Match(text, "[A-Za-z]").Value;

            if (officeSymbol.ToLower().Equals("o").ToString().Length == 1)
                return officeSymbol;

            switch (officeSymbol.ToLower())
            {
                case "o":
                    return "A";

                case "w":
                    return "P";

                default:
                    return officeSymbol;
            }
        }
    }

    internal sealed class LawEnforcementOfficerConverter : BooleanConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            switch (text.ToLower())
            {
                case "5":
                    return true;

                case "n":
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
            switch (text.ToLower())
            {
                case "0":
                    return "CO";

                case "a":
                    return "10";

                case "w":
                    return "NCR";

                default:
                    return text.PadLeft(2, '0');
            }
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
}