using System.Collections.Generic;

namespace HRUpdate.Lookups
{
    internal class Lookup
    {
        public List<Region_Lookup> region_lookup { get; set; }
        public List<Investigation_Lookup> investigation_lookup { get; set; }
    }
}