using HRUpdate.Models;
using HRUpdate.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;

namespace HRUpdate.Data
{
    internal class SaveData
    {
        //Reference to logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Set up connection
        private readonly MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GCIMS"].ToString());

        private readonly MySqlCommand cmd = new MySqlCommand();

        public SaveData()
        {

        }

        public string InsertEmployeeID(Int64 persID, Int64 employeeID)
        {
            try
            {
                using (conn)
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    using (cmd)
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "HR_InsertEmplID";

                        cmd.Parameters.Clear();

                        MySqlParameter[] personParameters = new MySqlParameter[]
                        {
                            new MySqlParameter { ParameterName = "persID", Value = persID, MySqlDbType = MySqlDbType.Int64},
                            new MySqlParameter { ParameterName = "emplID", Value = employeeID, MySqlDbType = MySqlDbType.VarChar, Size = 12},
                            new MySqlParameter { ParameterName = "SQLExceptionWarning", MySqlDbType=MySqlDbType.VarChar, Size=4000, Direction = ParameterDirection.Output },
                        };

                        cmd.Parameters.AddRange(personParameters);

                        cmd.ExecuteNonQuery();

                        return cmd.Parameters["SQLExceptionWarning"].Value.ToString();
                    }
                }
            }
            catch(Exception ex)
            {
                log.Error("Updating EmployeeID: " + ex.Message + " - " + ex.InnerException);
                return ex.Message.ToString();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="saveData"></param>
        /// <returns></returns>
        /// Change to person data
        public Tuple<int, string, string> UpdatePersonInformation(Int64 persID, Employee hrData)
        {
            Helpers helper = new Helpers();
           
            try
            {
                using (conn)
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    using (cmd)
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "HR_UpdatePerson";

                        cmd.Parameters.Clear();
                        
                        MySqlParameter[] personParameters = new MySqlParameter[]
                        {
                            new MySqlParameter { ParameterName = "persID", Value = persID, MySqlDbType = MySqlDbType.Int64},
                            new MySqlParameter { ParameterName = "emplID", Value = hrData.Person.EmployeeID, MySqlDbType = MySqlDbType.VarChar, Size = 11},
                            new MySqlParameter { ParameterName = "SSN", Value = hrData.Person.SocialSecurityNumber, MySqlDbType = MySqlDbType.TinyBlob },
                            new MySqlParameter { ParameterName = "HashedSSN", Value = helper.HashSSN(hrData.Person.SocialSecurityNumber), MySqlDbType = MySqlDbType.Binary, Size = 32 },
                            new MySqlParameter { ParameterName = "HashedSSNLast4", Value = helper.HashSSN(hrData.Person.SocialSecurityNumber.Substring(5,4)), MySqlDbType = MySqlDbType.Binary, Size = 32 },
                            new MySqlParameter { ParameterName = "cityOfBirth", Value = hrData.Birth.CityOfBirth, MySqlDbType = MySqlDbType.TinyBlob},
                            new MySqlParameter { ParameterName = "stateOfBirth", Value = hrData.Birth.StateOfBirth, MySqlDbType = MySqlDbType.TinyBlob},
                            new MySqlParameter { ParameterName = "countryOfBirth", Value = hrData.Birth.CountryOfBirth, MySqlDbType = MySqlDbType.TinyBlob},
                            new MySqlParameter { ParameterName = "countryOfCitizenship", Value = hrData.Birth.CountryOfCitizenship, MySqlDbType = MySqlDbType.VarChar, Size = 2},
                            new MySqlParameter { ParameterName = "isCitizen", Value = hrData.Birth.Citizen, MySqlDbType = MySqlDbType.Byte},
                            new MySqlParameter { ParameterName = "homeAddress1", Value = hrData.Address.HomeAddress1, MySqlDbType = MySqlDbType.TinyBlob},
                            new MySqlParameter { ParameterName = "homeAddress2", Value = hrData.Address.HomeAddress2, MySqlDbType = MySqlDbType.TinyBlob},
                            new MySqlParameter { ParameterName = "homeCity", Value = hrData.Address.HomeCity, MySqlDbType = MySqlDbType.VarChar, Size = 50},
                            new MySqlParameter { ParameterName = "homeState", Value = hrData.Address.HomeState, MySqlDbType = MySqlDbType.VarChar, Size = 2},
                            new MySqlParameter { ParameterName = "homeZipCode", Value = hrData.Address.HomeZipCode, MySqlDbType = MySqlDbType.VarChar, Size = 10},
                            new MySqlParameter { ParameterName = "homeCountry", Value = hrData.Address.HomeCountry, MySqlDbType = MySqlDbType.VarChar, Size = 2},
                            new MySqlParameter { ParameterName = "dateOfBirth", Value = hrData.Birth.DateOfBirth?.ToString("yyyy-M-dd"), MySqlDbType = MySqlDbType.TinyBlob},
                            new MySqlParameter { ParameterName = "gender", Value = hrData.Person.Gender, MySqlDbType = MySqlDbType.VarChar, Size = 1},
                            new MySqlParameter { ParameterName = "scdLeave", Value = hrData.Person.ServiceComputationDateLeave?.ToString("yyyy-M-dd"), MySqlDbType = MySqlDbType.Date},
                            new MySqlParameter { ParameterName = "priorInvestigation", Value = hrData.Investigation.PriorInvestigation, MySqlDbType = MySqlDbType.VarChar, Size = 20},
                            new MySqlParameter { ParameterName = "typeOfInvestigation", Value = hrData.Investigation.TypeOfInvestigation, MySqlDbType = MySqlDbType.VarChar, Size = 20},
                            new MySqlParameter { ParameterName = "dateOfInvestigation", Value = hrData.Investigation.DateOfInvestigation?.ToString("yyyy-M-dd"), MySqlDbType = MySqlDbType.Date},
                            new MySqlParameter { ParameterName = "typeOfInvestigationToRequest", Value = hrData.Investigation.TypeOfInvestigationToRequest, MySqlDbType = MySqlDbType.VarChar, Size = 12},
                            new MySqlParameter { ParameterName = "initialResult", Value = hrData.Investigation.InitialResult, MySqlDbType = MySqlDbType.Byte},
                            new MySqlParameter { ParameterName = "initialResultDate", Value = hrData.Investigation.InitialResultDate?.ToString("yyyy-M-dd"), MySqlDbType = MySqlDbType.Date},
                            new MySqlParameter { ParameterName = "finalResult", Value = hrData.Investigation.FinalResult, MySqlDbType = MySqlDbType.Byte},
                            new MySqlParameter { ParameterName = "finalResultDate", Value = hrData.Investigation.FinalResultDate?.ToString("yyyy-M-dd"), MySqlDbType = MySqlDbType.Date},
                            new MySqlParameter { ParameterName = "adjudicatorEmplID", Value = hrData.Investigation.AdjudicatorEmployeeID, MySqlDbType = MySqlDbType.VarChar, Size = 11},
                            new MySqlParameter { ParameterName = "pcn", Value = hrData.Position.PositionControlNumber, MySqlDbType = MySqlDbType.VarChar, Size = 15},
                            new MySqlParameter { ParameterName = "jobTitle", Value = hrData.Person.JobTitle, MySqlDbType = MySqlDbType.VarChar, Size = 70},
                            new MySqlParameter { ParameterName = "supervisoryStatus", Value = hrData.Position.SupervisoryStatus, MySqlDbType = MySqlDbType.VarChar, Size = 2},
                            new MySqlParameter { ParameterName = "payPlan", Value = hrData.Position.PayPlan, MySqlDbType = MySqlDbType.VarChar, Size = 3},
                            new MySqlParameter { ParameterName = "jobSeries", Value = hrData.Position.JobSeries, MySqlDbType = MySqlDbType.VarChar, Size = 8},
                            new MySqlParameter { ParameterName = "payGrade", Value = hrData.Position.PayGrade, MySqlDbType = MySqlDbType.VarChar, Size = 3},
                            new MySqlParameter { ParameterName = "workSchedule", Value = hrData.Position.WorkSchedule, MySqlDbType = MySqlDbType.VarChar, Size = 1},
                            new MySqlParameter { ParameterName = "fero", Value = hrData.Person.FederalEmergencyResponseOfficial, MySqlDbType = MySqlDbType.Byte},
                            new MySqlParameter { ParameterName = "leo", Value = hrData.Person.LawEnforcementOfficer, MySqlDbType = MySqlDbType.Byte},
                            new MySqlParameter { ParameterName = "positionTeleworkEligibility", Value = hrData.Position.PositionTeleworkEligibility, MySqlDbType = MySqlDbType.Byte},
                            new MySqlParameter { ParameterName = "positionSensitivity", Value = hrData.Position.PositionSensitivity, MySqlDbType = MySqlDbType.VarChar, Size = 4},
                            new MySqlParameter { ParameterName = "positionStartDate", Value = hrData.Position.PositionStartDate?.ToString("yyyy-M-dd"), MySqlDbType = MySqlDbType.Date},
                            new MySqlParameter { ParameterName = "region", Value = hrData.Person.Region, MySqlDbType = MySqlDbType.VarChar, Size = 3},
                            new MySqlParameter { ParameterName = "dutyLocationCode", Value = hrData.Position.DutyLocationCode, MySqlDbType = MySqlDbType.VarChar, Size = 9},
                            new MySqlParameter { ParameterName = "dutyLocationCity", Value = hrData.Position.DutyLocationCity, MySqlDbType = MySqlDbType.VarChar, Size = 40},
                            new MySqlParameter { ParameterName = "dutyLocationState", Value = hrData.Position.DutyLocationState, MySqlDbType = MySqlDbType.VarChar, Size = 2},
                            new MySqlParameter { ParameterName = "dutyLocationCounty", Value = hrData.Position.DutyLocationCounty, MySqlDbType = MySqlDbType.VarChar, Size = 40},
                            new MySqlParameter { ParameterName = "agencyCodeSubelement", Value = hrData.Position.AgencyCodeSubelement, MySqlDbType = MySqlDbType.VarChar, Size = 4},
                            new MySqlParameter { ParameterName = "organizationCode", Value = hrData.Person.OrganizationCode, MySqlDbType = MySqlDbType.VarChar, Size = 2},
                            new MySqlParameter { ParameterName = "supervisorEmplId", Value = hrData.Position.SupervisorEmployeeID, MySqlDbType = MySqlDbType.Int64},
                            new MySqlParameter { ParameterName = "homeAddress3", Value = hrData.Address.HomeAddress3, MySqlDbType = MySqlDbType.TinyBlob},
                            //new MySqlParameter { ParameterName = "positionTitle", Value = hrData.Position.PositionTitle, MySqlDbType = MySqlDbType.VarChar, Size = 60},
                            new MySqlParameter { ParameterName = "positionOrganization", Value = hrData.Position.PositionOrganization, MySqlDbType = MySqlDbType.VarChar, Size = 18},
                            new MySqlParameter { ParameterName = "homePhone", Value = hrData.Phone.HomePhone, MySqlDbType = MySqlDbType.TinyBlob},
                            new MySqlParameter { ParameterName = "homeCell", Value = hrData.Phone.HomeCell, MySqlDbType = MySqlDbType.TinyBlob},
                            new MySqlParameter { ParameterName = "homeEmail", Value = hrData.Person.HomeEmail, MySqlDbType = MySqlDbType.TinyBlob},
                            new MySqlParameter { ParameterName = "emergencyContactName", Value = hrData.Emergency.EmergencyContactName, MySqlDbType = MySqlDbType.TinyBlob},
                            new MySqlParameter { ParameterName = "emergencyContactHomePhone", Value = hrData.Emergency.EmergencyContactHomePhone, MySqlDbType = MySqlDbType.TinyBlob},
                            new MySqlParameter { ParameterName = "emergencyContactWorkPhone", Value = hrData.Emergency.EmergencyContactWorkPhone, MySqlDbType = MySqlDbType.TinyBlob},
                            new MySqlParameter { ParameterName = "emergencyContactCellPhone", Value = hrData.Emergency.EmergencyContactCellPhone, MySqlDbType = MySqlDbType.TinyBlob},
                            new MySqlParameter { ParameterName = "outOfAreaContactName", Value = hrData.Emergency.OutOfAreaContactName, MySqlDbType = MySqlDbType.TinyBlob},
                            new MySqlParameter { ParameterName = "outOfAreaContactHomePhone", Value = hrData.Emergency.OutOfAreaContactHomePhone, MySqlDbType = MySqlDbType.TinyBlob},
                            new MySqlParameter { ParameterName = "outOfAreaContactWorkPhone", Value = hrData.Emergency.OutOfAreaContactWorkPhone, MySqlDbType = MySqlDbType.TinyBlob},
                            new MySqlParameter { ParameterName = "outOfAreaContactCellPhone", Value = hrData.Emergency.OutOfAreaContactCellPhone, MySqlDbType = MySqlDbType.TinyBlob},
                            new MySqlParameter { ParameterName = "workPhone", Value = hrData.Phone.WorkPhone, MySqlDbType = MySqlDbType.VarChar, Size = 22},
                            new MySqlParameter { ParameterName = "workFax", Value = hrData.Phone.WorkFax, MySqlDbType = MySqlDbType.VarChar, Size = 22},
                            new MySqlParameter { ParameterName = "workCell", Value = hrData.Phone.WorkCell, MySqlDbType = MySqlDbType.VarChar, Size = 22},
                            new MySqlParameter { ParameterName = "workTTY", Value = hrData.Phone.WorkTextTelephone, MySqlDbType = MySqlDbType.VarChar, Size = 22},
                            new MySqlParameter { ParameterName = "result", MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Output},
                            new MySqlParameter { ParameterName = "actionMsg", MySqlDbType = MySqlDbType.VarChar, Size = 50, Direction = ParameterDirection.Output },
                            new MySqlParameter { ParameterName = "SQLExceptionWarning", MySqlDbType=MySqlDbType.VarChar, Size=4000, Direction = ParameterDirection.Output },
                        };

                        cmd.Parameters.AddRange(personParameters);

                        cmd.ExecuteNonQuery();

                        return new Tuple<int, string, string>((int)cmd.Parameters["result"].Value, cmd.Parameters["actionMsg"].Value.ToString(), cmd.Parameters["SQLExceptionWarning"].Value.ToString());
                    }
                }
            }
            //Catch all errors
            catch (Exception ex)
            {
                log.Error("Updating GCIMS Record: " + ex.Message + " - " + ex.InnerException);
                return new Tuple<int, string, string>(-1, "-1", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Saves Separation Data
        /// </summary>
        /// <param name="separationData"></param>
        /// <returns></returns>
        public Tuple<int, int, string, string> SeparateUser(Separation separationData)
        {
            try
            {
                using (conn)
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    using (cmd)
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "HR_Separation";

                        cmd.Parameters.Clear();
                        MySqlParameter[] employeeParameters = new MySqlParameter[]
                        {
                            new MySqlParameter { ParameterName = "emplID", Value = separationData.EmployeeID, MySqlDbType = MySqlDbType.Int32},
                            new MySqlParameter { ParameterName = "separationReasonCode", Value = separationData.SeparationCode, MySqlDbType = MySqlDbType.VarChar, Size = 3},
                            new MySqlParameter { ParameterName = "separationDate", Value = separationData.SeparationDate, MySqlDbType = MySqlDbType.Date},
                            new MySqlParameter { ParameterName = "persID", MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Output},
                            new MySqlParameter { ParameterName = "result", MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Output},
                            new MySqlParameter { ParameterName = "actionMsg", MySqlDbType = MySqlDbType.VarChar, Size = 50, Direction = ParameterDirection.Output },
                            new MySqlParameter { ParameterName = "SQLExceptionWarning", MySqlDbType=MySqlDbType.VarChar, Size=4000, Direction = ParameterDirection.Output },
                        };

                        cmd.Parameters.AddRange(employeeParameters);

                        cmd.ExecuteNonQuery();

                        return new Tuple<int, int, string, string>((int)cmd.Parameters["persID"].Value, (int)cmd.Parameters["result"].Value, cmd.Parameters["actionMsg"].Value.ToString(), cmd.Parameters["SQLExceptionWarning"].Value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("SaveSeparationInformation: " + separationData.EmployeeID + " - " + ex.Message + " - " + ex.InnerException);
                return new Tuple<int, int, string, string>(-1, -1, "Unknown Error", "Unknown SQL Exception Warning");
            }
        }
    }
}