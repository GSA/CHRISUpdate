using CsvHelper.Configuration;
using HRUpdate.Models;

namespace HRUpdate.Mapping
{
    class OrganizationMapping { }

    sealed class CustomOrganizationMap : CsvClassMap<Organization>
    {
        public CustomOrganizationMap()
        {
            Map(m => m.AbolishedbyOrder).Index(OrganizationConstants.ABOLISHED_BY_ORDER);
            Map(m => m.AgencyCodeSublement).Index(OrganizationConstants.AGENCY_CODE_SUBLEMENT);
            Map(m => m.ChangedByOrder).Index(OrganizationConstants.CHANGED_BY_ORDER);
            Map(m => m.CreatedByOrder).Index(OrganizationConstants.CREATED_BY_ORDER);
            Map(m => m.DateAbolished).Index(OrganizationConstants.DATE_ABOLISHED);
            Map(m => m.DateOfLastChange).Index(OrganizationConstants.DATE_OF_LAST_CHANGE);
            Map(m => m.FromDate).Index(OrganizationConstants.FROM_DATE);
            Map(m => m.Location).Index(OrganizationConstants.LOCATION);
            Map(m => m.LocationAddress).Index(OrganizationConstants.LOCATION_ADDRESS);
            Map(m => m.Name).Index(OrganizationConstants.NAME);
            Map(m => m.OfficeSymbol).Index(OrganizationConstants.OFFICE_SYMBOL);
            Map(m => m.OrgClassificationsName).Index(OrganizationConstants.ORG_CLASSIFICATIONS_NAME);
            Map(m => m.OrgInfoLine1).Index(OrganizationConstants.ORGINFO_LINE_1);
            Map(m => m.OrgInfoLine2).Index(OrganizationConstants.ORGINFO_LINE_2);
            Map(m => m.OrgInfoLine3).Index(OrganizationConstants.ORGINFO_LINE_3);
            Map(m => m.OrgInfoLine4).Index(OrganizationConstants.ORGINFO_LINE_4);
            Map(m => m.OrgInfoLine5).Index(OrganizationConstants.ORGINFO_LINE_5);
            Map(m => m.OrgInfoLine6).Index(OrganizationConstants.ORGINFO_LINE_6);
            Map(m => m.PersonnelOfficeIdentifier).Index(OrganizationConstants.PERSONNEL_OFFICE_IDENTIFIER);
            Map(m => m.ToDate).Index(OrganizationConstants.TO_DATE);
            Map(m => m.OCTOrgTitle).Index(OrganizationConstants.OCT_ORG_TITLE);
            Map(m => m.OPMOrganizationalComponent).Index(OrganizationConstants.OPM_ORGANIZATIONAL_COMPONENT);
            Map(m => m.OrgClassificationsEnabled).Index(OrganizationConstants.ORG_CLASSIFICATIONS_ENABLED);
        }
    }
}
