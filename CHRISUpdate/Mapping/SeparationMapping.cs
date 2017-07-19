using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CHRISUpdate.Models;
using CsvHelper.Configuration;

namespace CHRISUpdate.Mapping
{
    //class SeparationMapping { }

    sealed class CustomSeparationMap : CsvClassMap<Separation>
    {
        public CustomSeparationMap()
        {
            Map(m => m.EmployeeUniqueID).Index(SeparationConstants.UNIQUE_ID);
            Map(m => m.EmployeeID).Index(SeparationConstants.CHRIS_ID);
            Map(m => m.LastNameAndSuffix).Index(SeparationConstants.LAST_NAME_AND_SUFFIX);
            Map(m => m.FirstName).Index(SeparationConstants.FIRST_NAME);
            Map(m => m.MiddleName).Index(SeparationConstants.MIDDLE_NAME);
            Map(m => m.PreferredFirstName).Index(SeparationConstants.PREFERRED_NAME);
            Map(m => m.SSN).Index(SeparationConstants.SSN);
            Map(m => m.SeparationCode).Index(SeparationConstants.SEPARATION_CODE);
            Map(m => m.SeparationDate).Index(SeparationConstants.SEPARATION_DATE);
        }
    }
}
