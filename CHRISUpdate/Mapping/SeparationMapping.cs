using CsvHelper.Configuration;
using HRUpdate.Models;

namespace HRUpdate.Mapping
{
    sealed class CustomSeparationMap : ClassMap<Separation>
    {
        public CustomSeparationMap()
        {            
            Map(m => m.EmployeeID).Index(SeparationConstants.EMPLOYEE_ID);
            Map(m => m.SeparationCode).Index(SeparationConstants.SEPARATION_CODE);
            Map(m => m.SeparationDate).Index(SeparationConstants.SEPARATION_DATE);
        }
    }
}
