using System;

namespace HRUpdate.Models
{
    internal class EMailData
    {
        public string HRFilename { get; set; }
        public string SEPFileName { get; set; }

        public Int64 HRFailed { get; set; }
        public Int64 HRSocial { get; set; }
        public Int64 HRInactive { get; set; }
        public Int64 HRIdentical { get; set; }
        public Int64 HRAttempted { get; set; }
        public Int64 HRSucceeded { get; set; }
        public Int64 HRRecordsNotFound { get; set; }

        public string HRSuccessfulFilename { get; set; }
        public string HRUnsuccessfulFilename { get; set; }
        public string HRIdenticalFilename { get; set; }
        public string HRSocialSecurityNumberChangeFilename { get; set; }
        public string HRInactiveFilename { get; set; }
        public string HRRecordsNotFoundFileName { get; set; }

        public bool HRHasErrors { get; set; }

        public Int64 SEPAttempted { get; set; }
        public Int64 SEPSucceeded { get; set; }
        public Int64 SEPFailed { get; set; }
        public Int64 SEPTHCount { get; set; }//#148
        public Int64 SEPRecCount { get; set; }//#148

        public string SeparationSuccessfulFilename { get; set; }
        public string SeparationErrorFilename { get; set; }

        public bool SEPHasErrors { get; set; }
        public bool SEPHasTHErrors { get; set; }//#148
    }
}