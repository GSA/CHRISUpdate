using System.Collections.Generic;

namespace HRUpdate.Lookups
{
    internal class Lookup
    {
        public List<RegionLookup> region_lookup { get; set; }
        public List<InvestigationLookup> investigation_lookup { get; set; }
        public List<SeparationLookup> separation_lookup { get; set; }
    }
}