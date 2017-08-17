﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CHRISUpdate.Models
{
    class Separation
    {
        public string EmployeeUniqueID { get; set; }
        public string EmployeeID { get; set; }
        public string LastNameAndSuffix { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string PreferredFirstName { get; set; }
        public string SSN { get; set; }
        public string SeparationCode { get; set; }
        public DateTime? SeparationDate { get; set; }
    }
}
