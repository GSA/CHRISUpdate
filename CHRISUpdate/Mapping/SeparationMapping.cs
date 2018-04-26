using AutoMapper;
using CsvHelper.Configuration;
using HRUpdate.Lookups;
using HRUpdate.Models;
using HRUpdate.Process;

namespace HRUpdate.Mapping
{
    internal sealed class SeparationMapping : ClassMap<Separation>
    {
        public SeparationMapping()
        {
            Map(m => m.EmployeeID).Index(SeparationConstants.EMPLOYEE_ID);
            Map(m => m.SeparationCode).Index(SeparationConstants.SEPARATION_CODE);
            Map(m => m.SeparationDate).Index(SeparationConstants.SEPARATION_DATE);
        }
    }
}