﻿using System.Collections.Generic;

namespace HRUpdate.Lookups
{
    public class Lookup
    {
        public List<RegionLookup> regionLookup { get; set; }

        public List<InvestigationLookup> investigationLookup { get; set; }

        public List<SeparationLookup> separationLookup { get; set; }

        public List<CountryLookup> countryLookup { get; set; }

        public List<StateLookup> stateLookup { get; set; }

        public List<BuildingLookup> BuildingLookup { get; set; }
    }
}