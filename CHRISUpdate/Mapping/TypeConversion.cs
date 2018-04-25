using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using HRUpdate.Lookups;
using HRUpdate.Process;
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
                    return null;
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
                    return null;
            }
        }
    }

    internal sealed class InvistigationResultConverter : BooleanConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            switch(text)
            {
                case "1":
                    return true;
                case "0":
                    return false;
                default:
                    return null;
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
            return investigationLookup.Where(w => w.Code == text).Select(s => s.Tier).SingleOrDefault();
        }
    }
}