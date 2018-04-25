using System.Collections.Generic;

namespace HRUpdate.Lookups
{
    public class Lookup
    {
        public List<RegionLookup> regionLookup { get; set; }

        public List<InvestigationLookup> investigationLookup { get; set; }

        public List<SeparationLookup> separationLookup { get; set; }
    }
}