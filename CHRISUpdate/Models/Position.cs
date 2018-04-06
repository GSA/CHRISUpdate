﻿using System;

namespace HRUpdate.Models
{
    public class Position
    {
        public string PositionControlNumber { get; set; }
        public string PositionTitle { get; set; }
        public string PositionOrganization { get; set; }
        public string SupervisoryStatus { get; set; }
        public string PayPlan { get; set; }
        public string JobSeries { get; set; }
        public string PayGrade { get; set; }
        public string WorkSchedule { get; set; }
        public string PositionTeleworkEligibility { get; set; }
        public string PositionSensitivity { get; set; }
        public DateTime? PositionStartDate { get; set; }
        public string AgencyCodeSubelement { get; set; }
        public string SupervisorEmployeeID { get; set; }
    }
}