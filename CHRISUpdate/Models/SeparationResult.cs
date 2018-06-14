using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRUpdate.Models
{
    class SeparationResult
    {
        public int GCIMSID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Action { get; set; }
        public int Result { get; set; }
        public string SqlError { get; set; }
    }
}
