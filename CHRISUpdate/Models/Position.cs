﻿using System;

namespace HRUpdate.Models
{
    public class Position
    {
        public string PositionControlNumber { get; set; }

        /// <summary>
        /// Office Symbol
        /// </summary>
        public string PositionOrganization { get; set; }

        public string SupervisoryStatus { get; set; }
        public string PayPlan { get; set; }
        public string JobSeries { get; set; }
        public string PayGrade { get; set; }
        public string WorkSchedule { get; set; }
        public bool? PositionTeleworkEligibility { get; set; }
        public string PositionSensitivity { get; set; }
        public string DutyLocationCode { get; set; }
        public string DutyLocationCity { get; set; }
        public string DutyLocationState { get; set; }
        public string DutyLocationCounty { get; set; }
        public DateTime? PositionStartDate { get; set; }
        public string AgencyCodeSubelement { get; set; }
        public string SupervisorEmployeeID { get; set; }
    }
}