using HRUpdate.Models;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;

namespace HRUpdate.Process
{
    class SaveData
    {
        //Reference to logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Set up connection
        private readonly MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GCIMS"].ToString());
        private readonly MySqlCommand cmd = new MySqlCommand();              

        //Empty Contructor
        public SaveData(){}

        /// <summary>
        /// Saves chris info and returns true if successful
        /// </summary>
        /// <param name="saveData"></param>
        /// <param name="conn"></param>
        /// <returns>bool</returns>
        public bool SaveCHRISInformation(HR saveData)
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

                        AddOrUpdate(saveData.Employee);
                        AddOrUpdate(saveData.Position);
                        AddOrUpdate(saveData.Supervisor);
                        AddOrUpdate(saveData.Security);
                        AddOrUpdate(saveData.Person);

                        return true;
                    }

                }               
            }
            //Catch all errors
            catch (Exception ex)
            {
                //Log error and return false for failure while saving
                log.Warn("[SaveCHRISInformation] - Unable to save " + saveData.ChrisID + " - " + ex.Message + " - " + ex.InnerException);
                return false;
            }
        }

        /// <summary>
        /// Saves Separation Data
        /// </summary>
        /// <param name="separationData"></param>
        /// <returns></returns>
        public Tuple<int, int, string>SaveSeparationInformation(Separation separationData)
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

                        //Clear sql parameters and create new sql parameters
                        cmd.Parameters.Clear();
                        MySqlParameter[] employeParameters = new MySqlParameter[] 
                        {
                            new MySqlParameter { ParameterName = "emplID", Value = separationData.EmployeeID, MySqlDbType = MySqlDbType.Int32},
                            new MySqlParameter { ParameterName = "separationReasonCode", Value = separationData.SeparationCode, MySqlDbType = MySqlDbType.VarChar, Size = 3},
                            new MySqlParameter { ParameterName = "separationDate", Value = separationData.SeparationDate, MySqlDbType = MySqlDbType.Date},
                            new MySqlParameter { ParameterName = "persID", MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Output},
                            new MySqlParameter { ParameterName = "result", MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Output},
                            new MySqlParameter { ParameterName = "SQLExceptionWarning", MySqlDbType=MySqlDbType.VarChar, Size=4000, Direction = ParameterDirection.Output },
                        };

                        cmd.Parameters.AddRange(employeParameters);
                       
                        cmd.ExecuteNonQuery();

                        return new Tuple<int, int, string>((int)cmd.Parameters["persID"].Value, (int)cmd.Parameters["result"].Value, cmd.Parameters["SQLExceptionWarning"].Value.ToString());
                    }

                }
            }
            catch (Exception ex)
            {
                log.Warn("[SaveSeparationInformation] Unable to save " + separationData.EmployeeID + " - " + ex.Message + " - " + ex.InnerException);
                return new Tuple<int, int, string>(-1, -1, "Unknown Error");
            }                    
        }
       
        /// <summary>
        /// Add or update person data
        /// </summary>
        /// <param name="personData"></param>
        private void AddOrUpdate(Person personData)
        {
            try
            {
                //Set up cmd
                cmd.CommandText = "Person";

                //Clear sql params
                cmd.Parameters.Clear();

                //New sql params
                MySqlParameter[] personParameters = new MySqlParameter[] {
                                        new MySqlParameter { ParameterName = "@PersonID", Value = personData.PersonID, MySqlDbType = MySqlDbType.Int32, Size = 20 },
                                        new MySqlParameter { ParameterName = "@Gender", Value = personData.Gender, MySqlDbType = MySqlDbType.VarChar, Size = 1 },
                                        new MySqlParameter { ParameterName = "@SupervisoryLevel", Value = personData.SupervisoryLevel, MySqlDbType = MySqlDbType.VarChar, Size = 2 },
                                        new MySqlParameter { ParameterName = "@ChrisID", Value = personData.ChrisID, MySqlDbType = MySqlDbType.VarChar, Size = 12 },
                                        new MySqlParameter { ParameterName = "@JobTitle", Value = personData.JobTitle, MySqlDbType = MySqlDbType.VarChar, Size = 60 },
                                        new MySqlParameter { ParameterName = "@OfficeSymbol", Value = personData.OfficeSymbol, MySqlDbType = MySqlDbType.VarChar, Size = 12 },
                                        new MySqlParameter { ParameterName = "@MajorOrg", Value = personData.MajorOrg, MySqlDbType = MySqlDbType.VarChar, Size = 2 },
                                        new MySqlParameter { ParameterName = "@Region", Value = personData.Region, MySqlDbType = MySqlDbType.VarChar, Size = 3 },
                                        new MySqlParameter { ParameterName = "@Supervisor", Value = personData.Supervisor, MySqlDbType = MySqlDbType.VarChar, Size = 100 },                                        
                                    };

                //Add new params to cmd
                cmd.Parameters.AddRange(personParameters);

                //Execute cmd
                cmd.ExecuteNonQuery();
            }
            //Catch all errors
            catch (Exception ex)
            {
                //Log Error and re-throw
                log.Error("[AddOrUpdate - Person] - Process Users Error:" + ex.Message + " " + ex.InnerException);
                throw;
            }
        }
                
        /// <summary>
        /// Add or update employee data
        /// </summary>
        /// <param name="employeeData"></param>
        private void AddOrUpdate(Employee employeeData)
        {
            try
            {
                //bool IsSavePII = false;

                //bool.TryParse(ConfigurationManager.AppSettings["LOADPII"].ToString(), out IsSavePII);

                //if (!IsSavePII) //Should always be false
                    //employeeData.SSN = null;

                //Set up cmd
                cmd.CommandText = "Employee";

                //Clear sql params
                cmd.Parameters.Clear();

                //Set sql parameters
                MySqlParameter[] employeParameters = new MySqlParameter[] {
                                        new MySqlParameter { ParameterName = "@PersonID", Value = employeeData.PersonID, MySqlDbType = MySqlDbType.Int32, Size = 20 },
                                        new MySqlParameter { ParameterName = "@EmpUniqueID", Value = employeeData.UniqueEmployeeID, MySqlDbType = MySqlDbType.VarChar, Size = 30 },
                                        new MySqlParameter { ParameterName = "@EmpID", Value = employeeData.EmployeeID, MySqlDbType = MySqlDbType.VarChar, Size = 15 },
                                        new MySqlParameter { ParameterName = "@SSN", Value = DBNull.Value, MySqlDbType = MySqlDbType.VarBinary, Size = 32 }, //No need to store this
                                        new MySqlParameter { ParameterName = "@AgencyCode", Value = employeeData.AgencyCode, MySqlDbType = MySqlDbType.VarChar, Size = 2 },
                                        new MySqlParameter { ParameterName = "@EmpStatus", Value = employeeData.EmployeeStatus, MySqlDbType = MySqlDbType.VarChar, Size = 8 },
                                        //new MySqlParameter { ParameterName = "@DutyStatus", Value = employeeData.DutyStatus, MySqlDbType = MySqlDbType.VarChar, Size = 2 },
                                        //new MySqlParameter { ParameterName = "@AssignmentStatus", Value = employeeData.AssignmentStatus, MySqlDbType = MySqlDbType.VarChar, Size = 80 },
                                        new MySqlParameter { ParameterName = "@SCDLeave", Value = employeeData.SCDLeave, MySqlDbType = MySqlDbType.Date },
                                        //new MySqlParameter { ParameterName = "@Suffix", Value = employeeData.FamilySuffix, MySqlDbType = MySqlDbType.VarChar, Size = 150 },
                                        new MySqlParameter { ParameterName = "@FirstName", Value = employeeData.FirstName, MySqlDbType = MySqlDbType.VarChar, Size = 150 },
                                        new MySqlParameter { ParameterName = "@MiddleName", Value = employeeData.MiddleName, MySqlDbType = MySqlDbType.VarChar, Size = 60 },
                                        new MySqlParameter { ParameterName = "@LastName", Value = employeeData.LastName, MySqlDbType = MySqlDbType.VarChar, Size = 60 },
                                        new MySqlParameter { ParameterName = "@Suffix", Value = employeeData.Suffix, MySqlDbType = MySqlDbType.VarChar, Size = 60 },
                                        new MySqlParameter { ParameterName = "@Gender", Value = employeeData.Gender, MySqlDbType = MySqlDbType.VarChar, Size = 1 },
                                        new MySqlParameter { ParameterName = "@SupervisoryStatus", Value = employeeData.SupervisoryStatus, MySqlDbType = MySqlDbType.VarChar, Size = 1 },
                                    };

                //Add parameters to cmd
                cmd.Parameters.AddRange(employeParameters);

                //Execute cmd
                cmd.ExecuteNonQuery();
            }
            //Catch all errors
            catch (Exception ex)
            {
                //Log errors and re-throw
                log.Error("[AddOrUpdate - Employee] - Process Users Error:" + ex.Message + " " + ex.InnerException);
                throw;
            }
        }

        /// <summary>
        /// add or update position data
        /// </summary>
        /// <param name="employeeData"></param>
        private void AddOrUpdate(Position employeeData)
        {
            try
            {
                //Setup cmd
                cmd.CommandText = "Position";

                //Clear sql params
                cmd.Parameters.Clear();

                //New sql params
                MySqlParameter[] employeParameters = new MySqlParameter[] {
                                        new MySqlParameter { ParameterName = "@EmployeeID", Value = employeeData.EmployeeID, MySqlDbType = MySqlDbType.VarChar, Size = 15 },
                                        //new MySqlParameter { ParameterName = "@PosNo", Value = employeeData.PositionNumber, MySqlDbType = MySqlDbType.VarChar, Size = 15 },
                                        new MySqlParameter { ParameterName = "@PositionControlNumber", Value = employeeData.PositionControlNumber, MySqlDbType = MySqlDbType.VarChar, Size = 15 },
                                        //new MySqlParameter { ParameterName = "@PositionControlNumberIndicator", Value = employeeData.PositionControlNumberIndicator, MySqlDbType = MySqlDbType.VarChar, Size = 1 },
                                        new MySqlParameter { ParameterName = "@AgencyCodeSubelment", Value = employeeData.AgencyCodeSubelment, MySqlDbType = MySqlDbType.VarChar, Size = 4 },
                                        new MySqlParameter { ParameterName = "@TeleworkEligible", Value = employeeData.TeleworkEligible, MySqlDbType = MySqlDbType.VarChar, Size = 1 },
                                        new MySqlParameter { ParameterName = "@Sensitivity", Value = employeeData.Sensitivity, MySqlDbType = MySqlDbType.VarChar, Size = 1 },
                                        new MySqlParameter { ParameterName = "@StartDate", Value = employeeData.StartDate, MySqlDbType = MySqlDbType.Date },
                                        new MySqlParameter { ParameterName = "@EndDate", Value = employeeData.EndDate, MySqlDbType = MySqlDbType.Date },
                                        new MySqlParameter { ParameterName = "@JobTitle", Value = employeeData.JobTitle, MySqlDbType = MySqlDbType.VarChar, Size = 60 },
                                        new MySqlParameter { ParameterName = "@OrganizationCode", Value = employeeData.OrganizationCode, MySqlDbType = MySqlDbType.VarChar, Size = 250 },
                                        new MySqlParameter { ParameterName = "@OfficeSymbol", Value = employeeData.OfficeSymbol, MySqlDbType = MySqlDbType.VarChar, Size = 18 },
                                        new MySqlParameter { ParameterName = "@PayPlan", Value = employeeData.PayPlan, MySqlDbType = MySqlDbType.VarChar, Size = 2 },
                                        new MySqlParameter { ParameterName = "@JobSeries", Value = employeeData.JobSeries, MySqlDbType = MySqlDbType.VarChar, Size = 4 },
                                        new MySqlParameter { ParameterName = "@LevelGrade", Value = employeeData.LevelGrade, MySqlDbType = MySqlDbType.VarChar, Size = 2 },
                                        new MySqlParameter { ParameterName = "@WorkSchedule", Value = employeeData.WorkSchedule, MySqlDbType = MySqlDbType.VarChar, Size = 1 },
                                        new MySqlParameter { ParameterName = "@Region", Value = employeeData.Region, MySqlDbType = MySqlDbType.VarChar, Size = 3 },
                                        new MySqlParameter { ParameterName = "@DutyCode", Value = employeeData.DutyCode, MySqlDbType = MySqlDbType.VarChar, Size = 9 },
                                        new MySqlParameter { ParameterName = "@DutyCity", Value = employeeData.DutyCity, MySqlDbType = MySqlDbType.VarChar, Size = 40 },
                                        new MySqlParameter { ParameterName = "@DutyState", Value = employeeData.DutyState, MySqlDbType = MySqlDbType.VarChar, Size = 40 },
                                        new MySqlParameter { ParameterName = "@DutyCounty", Value = employeeData.DutyCounty, MySqlDbType = MySqlDbType.VarChar, Size = 40 },
                                        new MySqlParameter { ParameterName = "@IsDetail", Value = false, MySqlDbType = MySqlDbType.Bit },
                                    };

                //Add new params to cmd
                cmd.Parameters.AddRange(employeParameters);

                //Execute cmd
                cmd.ExecuteNonQuery();

                //Need to figure out a better way for parameters
                //If on detail than insert detail information
                if (employeeData.IsDetail)
                {
                    cmd.Parameters.Clear();

                    MySqlParameter[] employeParametersDetails = new MySqlParameter[] {
                                        new MySqlParameter { ParameterName = "@EmployeeID", Value = employeeData.EmployeeID, MySqlDbType = MySqlDbType.VarChar, Size = 15 },
                                        //new MySqlParameter { ParameterName = "@PosNo", Value = employeeData.DetailPositionNumber, MySqlDbType = MySqlDbType.VarChar, Size = 15 },
                                        new MySqlParameter { ParameterName = "@PositionControlNumber", Value = employeeData.DetailPositionControlNumber, MySqlDbType = MySqlDbType.VarChar, Size = 15 },
                                        //new MySqlParameter { ParameterName = "@PositionControlNumberIndicator", Value = employeeData.DetailPositionControlNumberIndicator, MySqlDbType = MySqlDbType.VarChar, Size = 1 },
                                        new MySqlParameter { ParameterName = "@AgencyCodeSubelment", Value = employeeData.AgencyCodeSubelment, MySqlDbType = MySqlDbType.VarChar, Size = 4 },
                                        new MySqlParameter { ParameterName = "@TeleworkEligible", Value = employeeData.TeleworkEligible, MySqlDbType = MySqlDbType.VarChar, Size = 1 },
                                        new MySqlParameter { ParameterName = "@Sensitivity", Value = employeeData.Sensitivity, MySqlDbType = MySqlDbType.VarChar, Size = 1 },
                                        new MySqlParameter { ParameterName = "@StartDate", Value = employeeData.DetailBeginDate, MySqlDbType = MySqlDbType.Date },
                                        new MySqlParameter { ParameterName = "@EndDate", Value = employeeData.DetailEndDate, MySqlDbType = MySqlDbType.Date },
                                        new MySqlParameter { ParameterName = "@JobTitle", Value = employeeData.DetailPositionTitle, MySqlDbType = MySqlDbType.VarChar, Size = 60 },
                                        new MySqlParameter { ParameterName = "@OrganizationCode", Value = employeeData.DetailOrganizationCode, MySqlDbType = MySqlDbType.VarChar, Size = 250 },
                                        new MySqlParameter { ParameterName = "@OfficeSymbol", Value = employeeData.DetailOfficeSymbol, MySqlDbType = MySqlDbType.VarChar, Size = 18 },
                                        new MySqlParameter { ParameterName = "@PayPlan", Value = employeeData.DetailPayPlan, MySqlDbType = MySqlDbType.VarChar, Size = 2 },
                                        new MySqlParameter { ParameterName = "@JobSeries", Value = employeeData.DetailJobSeries, MySqlDbType = MySqlDbType.VarChar, Size = 4 },
                                        new MySqlParameter { ParameterName = "@LevelGrade", Value = employeeData.DetailLevelGrade, MySqlDbType = MySqlDbType.VarChar, Size = 2 },
                                        new MySqlParameter { ParameterName = "@WorkSchedule", Value = employeeData.DetailWorkSchedule, MySqlDbType = MySqlDbType.VarChar, Size = 1 },
                                        new MySqlParameter { ParameterName = "@Region", Value = employeeData.DetailRegion, MySqlDbType = MySqlDbType.VarChar, Size = 3 },
                                        new MySqlParameter { ParameterName = "@DutyCode", Value = employeeData.DetailDutyCode, MySqlDbType = MySqlDbType.VarChar, Size = 9 },
                                        new MySqlParameter { ParameterName = "@DutyCity", Value = employeeData.DetailDutyCity, MySqlDbType = MySqlDbType.VarChar, Size = 40 },
                                        new MySqlParameter { ParameterName = "@DutyState", Value = employeeData.DetailDutyState, MySqlDbType = MySqlDbType.VarChar, Size = 40 },
                                        new MySqlParameter { ParameterName = "@DutyCounty", Value = employeeData.DetailDutyCounty, MySqlDbType = MySqlDbType.VarChar, Size = 40 },
                                        new MySqlParameter { ParameterName = "@IsDetail", Value = employeeData.IsDetail, MySqlDbType = MySqlDbType.Bit },
                                    };

                    cmd.Parameters.AddRange(employeParametersDetails);

                    cmd.ExecuteNonQuery();
                }
            }
            //Catch errors
            catch (Exception ex)
            {
                //Log and re-throw
                log.Error("[AddOrUpdate - Position] - Process Users Error:" + ex.Message + " " + ex.InnerException);
                throw;
            }
        }

        /// <summary>
        /// Add or update supervisor data
        /// </summary>
        /// <param name="employeeData"></param>
        private void AddOrUpdate(Supervisor employeeData)
        {
            try
            {
                //Set up cmd
                cmd.CommandText = "Supervisor";

                //Clear sql params
                cmd.Parameters.Clear();

                //New sql params
                MySqlParameter[] employeParameters = new MySqlParameter[] {
                                        new MySqlParameter { ParameterName = "@EmployeeID", Value = employeeData.EmployeeID, MySqlDbType = MySqlDbType.VarChar, Size = 15 },
                                        new MySqlParameter { ParameterName = "@UniqueEmployeeID", Value = employeeData.UniqueEmployeeID, MySqlDbType = MySqlDbType.VarChar, Size = 30, IsNullable = true },
                                        new MySqlParameter { ParameterName = "@SupervisorEmployeeID", Value = employeeData.SupervisorEmployeeID, MySqlDbType = MySqlDbType.VarChar, Size = 15, IsNullable = true },
                                        //new MySqlParameter { ParameterName = "@LastNameSuffix", Value = employeeData.LastNameSuffix, MySqlDbType = MySqlDbType.VarChar, Size = 150, IsNullable = true },
                                        //new MySqlParameter { ParameterName = "@FirstName", Value = employeeData.FirstName, MySqlDbType = MySqlDbType.VarChar, Size = 150, IsNullable = true },
                                        //new MySqlParameter { ParameterName = "@MiddleName", Value = employeeData.MiddleName, MySqlDbType = MySqlDbType.VarChar, Size = 60, IsNullable = true },                                        
                                        //new MySqlParameter { ParameterName = "@PositionControlNumber", Value = employeeData.PositionControlNumber, MySqlDbType = MySqlDbType.VarChar, Size = 15, IsNullable = true },                                        
                                        //new MySqlParameter { ParameterName = "@PositionControlNumberIndicator", Value = employeeData.PositionControlNumberIndicator, MySqlDbType = MySqlDbType.VarChar, Size = 1, IsNullable = true },                                        
                                        //new MySqlParameter { ParameterName = "@EMail", Value = employeeData.EMail, MySqlDbType = MySqlDbType.VarChar, Size = 100, IsNullable = true },                                        

                                    };

                //Add new sql params to cmd
                cmd.Parameters.AddRange(employeParameters);

                //Execute cmd
                cmd.ExecuteNonQuery();
            }
            //Catch all errors
            catch (Exception ex)
            {
                //Log and re-throw
                log.Error("[AddOrUpdate - Supervisor] - Process Users Error:" + ex.Message + " " + ex.InnerException);
                throw;
            }
        }

        /// <summary>
        /// Add or update security data
        /// </summary>
        /// <param name="employeeData"></param>
        /// Have to map ID to Chris ID for Ajducaitor Auth
        private void AddOrUpdate(Security employeeData)
        {
            try
            {
                //Set up cmd
                cmd.CommandText = "Security";

                //Clear sql params
                cmd.Parameters.Clear();

                //New sql params
                MySqlParameter[] employeParameters = new MySqlParameter[] {
                                        new MySqlParameter { ParameterName = "@EmployeeID", Value = employeeData.EmployeeID, MySqlDbType = MySqlDbType.VarChar, Size = 15 },
                                        new MySqlParameter { ParameterName = "@TypeCompleted", Value = employeeData.TypeCompleted, MySqlDbType = MySqlDbType.VarChar, Size = 30 },
                                        new MySqlParameter { ParameterName = "@DateCompleted", Value = employeeData.DateCompleted, MySqlDbType = MySqlDbType.Date },
                                        new MySqlParameter { ParameterName = "@PriorCompleted", Value = employeeData.PriorCompleted, MySqlDbType = MySqlDbType.VarChar, Size = 30 },
                                        new MySqlParameter { ParameterName = "@TypeRequested", Value = employeeData.TypeRequested, MySqlDbType = MySqlDbType.VarChar, Size = 30 },
                                        new MySqlParameter { ParameterName = "@AdjudicationAuth", Value = employeeData.AdjudicationAuth, MySqlDbType = MySqlDbType.VarChar, Size = 240 },                                        
                                    };

                //Add sql params to cmd
                cmd.Parameters.AddRange(employeParameters);

                //Execute cmd
                cmd.ExecuteNonQuery();
            }
            //Catch all errors
            catch (Exception ex)
            {
                //Log errors and re-throw
                log.Error("[AddOrUpdate - Security] - Process Users Error:" + ex.Message + " " + ex.InnerException);
                throw;
            }
        }       
    }
}