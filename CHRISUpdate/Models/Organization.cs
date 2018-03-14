using System;

namespace CHRISUpdate.Models
{
    class Organization
    {
        public string AbolishedbyOrder { get; set; }
        public string AgencyCodeSublement { get; set; }
        public string ChangedByOrder { get; set; }
        public string CreatedByOrder { get; set; }
        public DateTime? DateAbolished { get; set; }
        public DateTime? DateOfLastChange { get; set; }
        public DateTime? FromDate { get; set; }
        public string Location { get; set; }
        public string LocationAddress { get; set; }
        public string Name { get; set; }
        public string OfficeSymbol { get; set; }
        public string OPMOrganizationalComponent { get; set; }
        public string OrgInfoLine1 { get; set; }
        public string OrgInfoLine2 { get; set; }
        public string OrgInfoLine3 { get; set; }
        public string OrgInfoLine4 { get; set; }
        public string OrgInfoLine5 { get; set; }
        public string OrgInfoLine6 { get; set; }
        public string PersonnelOfficeIdentifier { get; set; }
        public DateTime? ToDate { get; set; }
        public string OCTOrgTitle { get; set; }
        public string OrgClassificationsName { get; set; }
        public string OrgClassificationsEnabled { get; set; }
    }
}
