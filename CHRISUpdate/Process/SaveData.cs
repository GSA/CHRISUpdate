using System;
using System.Configuration;
using System.Data;
using CHRISUpdate.Models;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Reflection;

namespace CHRISUpdate.Process
{
    class SaveData
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string connectionString = ConfigurationManager.ConnectionStrings["HR"].ToString();

        //MySqlConnection conn = new MySqlConnection(connectionString);
        MySqlCommand cmd = new MySqlCommand();

        //Want to turn this into the typeconverter in mapping
        Utilities.Utilities u = new Utilities.Utilities();

        public SaveData()
        {            
            
        }

        public bool SaveCHRISInformation(Chris saveData, MySqlConnection conn)
        {   
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;

                //AddOrUpdate<Employee>(saveData.Employee, "Employee");

                AddOrUpdate(saveData.Employee);
                AddOrUpdate(saveData.Position);
                AddOrUpdate(saveData.Supervisor);
                AddOrUpdate(saveData.Security);
                AddOrUpdate(saveData.Person);

                //AddOrUpdate<Employee>(saveData.Employee);

                return true;
            }
            catch (Exception ex)
            {
                log.Warn("[SaveCHRISInformation] - Unable to save " + saveData.ChrisID + ex.InnerException);
                return false;
            }
        }

        public bool SaveSeparationInformation(Separation saveData, MySqlConnection conn)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;

                //AddOrupdate<Separation>(saveData, "Separation");

                AddOrUpdate(saveData);

                return true;
            }
            catch (Exception)
            {
                log.Warn("[SaveSeparationInformation] Unable to save " + saveData.EmployeeID);
                return false;
            }
        }

        public bool SaveOrganizationInformation(Organization saveData)
        {
            //AddOrupdate<Organization>(saveData, "Organization");
            return true;
        }
        
        private void AddOrUpdate(Organization organizationData)
        {
            try
            {
                cmd.CommandText = "Organization";

                cmd.Parameters.Clear();

                MySqlParameter[] employeParameters = new MySqlParameter[] {
                                        new MySqlParameter { ParameterName = "@SeparationCode", Value = organizationData.AbolishedbyOrder, MySqlDbType = MySqlDbType.VarChar, Size = 2},
                                        new MySqlParameter { ParameterName = "@SeparationDate", Value = organizationData.ChangedByOrder, MySqlDbType = MySqlDbType.Date},
                                        new MySqlParameter { ParameterName = "@EmpID", Value = organizationData.ChangedByOrder, MySqlDbType = MySqlDbType.VarChar, Size = 15}, //This is CHRIS ID
                                    };

                cmd.Parameters.AddRange(employeParameters);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                log.Error("Process Organization Users Error:" + ex.Message + " " + ex.InnerException);
                throw;
            }
        }

        private void AddOrUpdate(Separation separationData)
        {
            try
            {
                cmd.CommandText = "Separation";

                cmd.Parameters.Clear();

                MySqlParameter[] employeParameters = new MySqlParameter[] {
                                        new MySqlParameter { ParameterName = "@SeparationCode", Value = separationData.SeparationCode, MySqlDbType = MySqlDbType.VarChar, Size = 2},
                                        new MySqlParameter { ParameterName = "@SeparationDate", Value = separationData.SeparationDate, MySqlDbType = MySqlDbType.Date},
                                        new MySqlParameter { ParameterName = "@EmpID", Value = separationData.EmployeeID, MySqlDbType = MySqlDbType.VarChar, Size = 15}, //This is CHRIS ID
                                    };

                cmd.Parameters.AddRange(employeParameters);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                log.Error("Process Separation Users Error:" + ex.Message + " " + ex.InnerException);
                throw;
            }
        }

        private void AddOrUpdate(Person personData)
        {
            try
            {
                cmd.CommandText = "Person";

                cmd.Parameters.Clear();

                MySqlParameter[] personParameters = new MySqlParameter[] {
                                        new MySqlParameter { ParameterName = "@PersonID", Value = personData.PersonID, MySqlDbType = MySqlDbType.Int32, Size = 20 },
                                        new MySqlParameter { ParameterName = "@Gender", Value = personData.Gender, MySqlDbType = MySqlDbType.VarChar, Size = 1 },
                                        new MySqlParameter { ParameterName = "@SupervisoryLevel", Value = personData.SupervisoryLevel, MySqlDbType = MySqlDbType.VarChar, Size = 2 },
                                        new MySqlParameter { ParameterName = "@ChrisID", Value = personData.ChrisID, MySqlDbType = MySqlDbType.VarChar, Size = 12 },
                                        new MySqlParameter { ParameterName = "@JobTitle", Value = personData.JobTitle, MySqlDbType = MySqlDbType.VarChar, Size = 60 },
                                        new MySqlParameter { ParameterName = "@OfficeSymbol", Value = personData.OfficeSymbol, MySqlDbType = MySqlDbType.VarChar, Size = 12 },
                                        //new MySqlParameter { ParameterName = "@MajorOrg", Value = personData.MajorOrg, MySqlDbType = MySqlDbType.VarChar, Size = 2 },
                                        new MySqlParameter { ParameterName = "@Region", Value = personData.Region, MySqlDbType = MySqlDbType.VarChar, Size = 3 },
                                        new MySqlParameter { ParameterName = "@Supervisor", Value = personData.Supervisor, MySqlDbType = MySqlDbType.VarChar, Size = 100 },                                        
                                    };

                cmd.Parameters.AddRange(personParameters);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                log.Error("[AddOrUpdate - Person] - Process Users Error:" + ex.Message + " " + ex.InnerException);
                throw;
            }
        }

        //auto set seperation values always to null here
        private void AddOrUpdate(Employee employeeData)
        {
            try
            {
                //bool IsSavePII = false;

                //bool.TryParse(ConfigurationManager.AppSettings["LOADPII"].ToString(), out IsSavePII);

                //if (!IsSavePII) //Should alwasy be false
                    //employeeData.SSN = null;

                cmd.CommandText = "Employee";

                cmd.Parameters.Clear();

                MySqlParameter[] employeParameters = new MySqlParameter[] {
                                        new MySqlParameter { ParameterName = "@PersonID", Value = employeeData.PersonID, MySqlDbType = MySqlDbType.Int32, Size = 20 },
                                        new MySqlParameter { ParameterName = "@EmpUniqueID", Value = employeeData.UniqueEmployeeID, MySqlDbType = MySqlDbType.VarChar, Size = 30 },
                                        new MySqlParameter { ParameterName = "@EmpID", Value = employeeData.EmployeeID, MySqlDbType = MySqlDbType.VarChar, Size = 15 },
                                        new MySqlParameter { ParameterName = "@SSN", Value = DBNull.Value, MySqlDbType = MySqlDbType.VarBinary, Size = 32 }, //No need to store this                                       
                                        new MySqlParameter { ParameterName = "@AgencyCode", Value = employeeData.AgencyCode, MySqlDbType = MySqlDbType.VarChar, Size = 2 },
                                        new MySqlParameter { ParameterName = "@EmpStatus", Value = employeeData.EmployeeStatus, MySqlDbType = MySqlDbType.VarChar, Size = 8 },
                                        new MySqlParameter { ParameterName = "@DutyStatus", Value = employeeData.DutyStatus, MySqlDbType = MySqlDbType.VarChar, Size = 2 },
                                        new MySqlParameter { ParameterName = "@AssignmentStatus", Value = employeeData.AssignmentStatus, MySqlDbType = MySqlDbType.VarChar, Size = 80 },
                                        new MySqlParameter { ParameterName = "@SCDLeave", Value = employeeData.SCDLeave, MySqlDbType = MySqlDbType.Date },
                                        new MySqlParameter { ParameterName = "@Suffix", Value = employeeData.FamilySuffix, MySqlDbType = MySqlDbType.VarChar, Size = 150 },
                                        new MySqlParameter { ParameterName = "@FirstName", Value = employeeData.FirstName, MySqlDbType = MySqlDbType.VarChar, Size = 150 },
                                        new MySqlParameter { ParameterName = "@MiddleName", Value = employeeData.MiddleName, MySqlDbType = MySqlDbType.VarChar, Size = 60 },
                                        new MySqlParameter { ParameterName = "@Gender", Value = employeeData.Gender, MySqlDbType = MySqlDbType.VarChar, Size = 1 },
                                        new MySqlParameter { ParameterName = "@SupervisoryStatus", Value = employeeData.SupervisoryStatus, MySqlDbType = MySqlDbType.VarChar, Size = 1 },                                        
                                    };

                cmd.Parameters.AddRange(employeParameters);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                log.Error("[AddOrUpdate - Employee] - Process Users Error:" + ex.Message + " " + ex.InnerException);
                throw;
            }
        }

        private void AddOrUpdate(Position employeeData)
        {
            try
            {
                cmd.CommandText = "Position";

                cmd.Parameters.Clear();

                MySqlParameter[] employeParameters = new MySqlParameter[] {
                                        new MySqlParameter { ParameterName = "@EmployeeID", Value = employeeData.EmployeeID, MySqlDbType = MySqlDbType.VarChar, Size = 15 },
                                        new MySqlParameter { ParameterName = "@PosNo", Value = employeeData.PositionNumber, MySqlDbType = MySqlDbType.VarChar, Size = 15 },
                                        new MySqlParameter { ParameterName = "@PositionControlNumber", Value = employeeData.PositionControlNumber, MySqlDbType = MySqlDbType.VarChar, Size = 15 },
                                        new MySqlParameter { ParameterName = "@PositionControlNumberIndicator", Value = employeeData.PositionControlNumberIndicator, MySqlDbType = MySqlDbType.VarChar, Size = 1 },
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

                cmd.Parameters.AddRange(employeParameters);

                cmd.ExecuteNonQuery();

                //Need to figure out a better way for parameters
                //If on detail than insert detail information
                if (employeeData.IsDetail)
                {
                    cmd.Parameters.Clear();

                    MySqlParameter[] employeParametersDetails = new MySqlParameter[] {
                                        new MySqlParameter { ParameterName = "@EmployeeID", Value = employeeData.EmployeeID, MySqlDbType = MySqlDbType.VarChar, Size = 15 },
                                        new MySqlParameter { ParameterName = "@PosNo", Value = employeeData.DetailPositionNumber, MySqlDbType = MySqlDbType.VarChar, Size = 15 },
                                        new MySqlParameter { ParameterName = "@PositionControlNumber", Value = employeeData.DetailPositionControlNumber, MySqlDbType = MySqlDbType.VarChar, Size = 15 },
                                        new MySqlParameter { ParameterName = "@PositionControlNumberIndicator", Value = employeeData.DetailPositionControlNumberIndicator, MySqlDbType = MySqlDbType.VarChar, Size = 1 },
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
            catch (Exception ex)
            {
                log.Error("[AddOrUpdate - Position] - Process Users Error:" + ex.Message + " " + ex.InnerException);
                throw;
            }
        }

        private void AddOrUpdate(Supervisor employeeData)
        {
            try
            {
                cmd.CommandText = "Supervisor";

                cmd.Parameters.Clear();

                MySqlParameter[] employeParameters = new MySqlParameter[] {
                                        new MySqlParameter { ParameterName = "@EmployeeID", Value = employeeData.EmployeeID, MySqlDbType = MySqlDbType.VarChar, Size = 15 },
                                        new MySqlParameter { ParameterName = "@UniqueEmployeeID", Value = employeeData.UniqueEmployeeID, MySqlDbType = MySqlDbType.VarChar, Size = 30, IsNullable = true },
                                        new MySqlParameter { ParameterName = "@SupervisorEmployeeID", Value = employeeData.SupervisorEmployeeID, MySqlDbType = MySqlDbType.VarChar, Size = 15, IsNullable = true },
                                        new MySqlParameter { ParameterName = "@LastNameSuffix", Value = employeeData.LastNameSuffix, MySqlDbType = MySqlDbType.VarChar, Size = 150, IsNullable = true },
                                        new MySqlParameter { ParameterName = "@FirstName", Value = employeeData.FirstName, MySqlDbType = MySqlDbType.VarChar, Size = 150, IsNullable = true },
                                        new MySqlParameter { ParameterName = "@MiddleName", Value = employeeData.MiddleName, MySqlDbType = MySqlDbType.VarChar, Size = 60, IsNullable = true },                                        
                                        new MySqlParameter { ParameterName = "@PositionControlNumber", Value = employeeData.PositionControlNumber, MySqlDbType = MySqlDbType.VarChar, Size = 15, IsNullable = true },                                        
                                        new MySqlParameter { ParameterName = "@PositionControlNumberIndicator", Value = employeeData.PositionControlNumberIndicator, MySqlDbType = MySqlDbType.VarChar, Size = 1, IsNullable = true },                                        
                                        new MySqlParameter { ParameterName = "@EMail", Value = employeeData.EMail, MySqlDbType = MySqlDbType.VarChar, Size = 100, IsNullable = true },                                        

                                    };

                cmd.Parameters.AddRange(employeParameters);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                log.Error("[AddOrUpdate - Supervisor] - Process Users Error:" + ex.Message + " " + ex.InnerException);
                throw;
            }
        }

        private void AddOrUpdate(Security employeeData)
        {
            try
            {
                cmd.CommandText = "Security";

                cmd.Parameters.Clear();

                MySqlParameter[] employeParameters = new MySqlParameter[] {
                                        new MySqlParameter { ParameterName = "@EmployeeID", Value = employeeData.EmployeeID, MySqlDbType = MySqlDbType.VarChar, Size = 15 },
                                        new MySqlParameter { ParameterName = "@TypeCompleted", Value = employeeData.TypeCompleted, MySqlDbType = MySqlDbType.VarChar, Size = 30 },
                                        new MySqlParameter { ParameterName = "@DateCompleted", Value = employeeData.DateCompleted, MySqlDbType = MySqlDbType.Date },
                                        new MySqlParameter { ParameterName = "@PriorCompleted", Value = employeeData.PriorCompleted, MySqlDbType = MySqlDbType.VarChar, Size = 30 },
                                        new MySqlParameter { ParameterName = "@TypeRequested", Value = employeeData.TypeRequested, MySqlDbType = MySqlDbType.VarChar, Size = 30 },
                                        new MySqlParameter { ParameterName = "@AdjudicationAuth", Value = employeeData.AdjudicationAuth, MySqlDbType = MySqlDbType.VarChar, Size = 240 },                                        
                                    };

                cmd.Parameters.AddRange(employeParameters);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                log.Error("[AddOrUpdate - Security] - Process Users Error:" + ex.Message + " " + ex.InnerException);
                throw;
            }
        }

        //need to figure out how to get this to work
        //private void AddOrUpdate<T>(T saveData, string commandText) where T : class
        //{
        //    try
        //    {
        //        cmd.CommandText = commandText;

        //        object result = Convert.ChangeType(saveData, typeof(T));

        //        PopulateParameters(result);

        //        cmd.ExecuteNonQuery();
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Error while trying to save data:" + ex.Message + " " + ex.InnerException);
        //        throw;
        //    }
        //}

        ////private void PopulateParamters<T>(T saveData) where T : Employee
        ////{

        ////}

        //private void PopulateParameters(Employee saveData)
        //{

        //}

        //private void PopulateParameters(Position saveData)
        //{

        //}

        //private void PopulateParameters<T>(T saveData) where T : class
        //{
        //    cmd.Parameters.Clear();

        //    MySqlParameter[] employeParameters = new MySqlParameter[] {
        //                                new MySqlParameter { ParameterName = "@SeparationCode", Value = organizationData.AbolishedbyOrder, MySqlDbType = MySqlDbType.VarChar, Size = 2},
        //                                new MySqlParameter { ParameterName = "@SeparationDate", Value = organizationData.ChangedByOrder, MySqlDbType = MySqlDbType.Date},
        //                                new MySqlParameter { ParameterName = "@EmpID", Value = organizationData.ChangedByOrder, MySqlDbType = MySqlDbType.VarChar, Size = 15}, //This is CHRIS ID
        //                            };

        //    cmd.Parameters.AddRange(employeParameters);

        //}
    }
}