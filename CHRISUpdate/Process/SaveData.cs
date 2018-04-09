using HRUpdate.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
        
        public Tuple<int, int, string> GetGCIMSRecord(string EmployeeID, int ssn, string lastName, string dateOfBirth)
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
                        cmd.CommandText = "HR_GetRecord";

                        cmd.Parameters.Clear();
                        MySqlParameter[] personParameters = new MySqlParameter[]
                        {
                            new MySqlParameter { ParameterName = "ssn", Value = ssn, MySqlDbType = MySqlDbType.Int32},
                            new MySqlParameter { ParameterName = "lastName", Value = lastName, MySqlDbType = MySqlDbType.VarChar, Size = 60},
                            new MySqlParameter { ParameterName = "dateOfBirth", Value = dateOfBirth, MySqlDbType = MySqlDbType.VarChar, Size = 10},
                            new MySqlParameter { ParameterName = "emplID", MySqlDbType = MySqlDbType.VarChar, Size = 12 },
                            new MySqlParameter { ParameterName = "persID", MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Output},
                            new MySqlParameter { ParameterName = "result", MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Output},
                            new MySqlParameter { ParameterName = "SQLExceptionWarning", MySqlDbType=MySqlDbType.VarChar, Size=4000, Direction = ParameterDirection.Output },
                        };

                        cmd.Parameters.AddRange(personParameters);

                        MySqlDataReader dr;
                        
                        dr = cmd.ExecuteReader();

                        List<Employee> gcimsData = new List<Employee>();
                        Person p = new Person();
                        Birth b = new Birth();

                        using (dr)
                        {
                            while (dr.Read())
                            {
                                p.FirstName = dr[1].ToString();
                                p.MiddleName = dr[2].ToString();
                                p.LastName = dr[3].ToString();

                                gcimsData.Add(new Employee
                                {
                                    Person = p,
                                    Birth = b
                                }
                                    );

                                Console.WriteLine("The Data: " + string.Format("{0}", dr[1]));
                            }
                        }

                        return new Tuple<int, int, string>((int)cmd.Parameters["persID"].Value, (int)cmd.Parameters["result"].Value, cmd.Parameters["SQLExceptionWarning"].Value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetGCIMSRecord: " + EmployeeID + " - " + ex.Message + " - " + ex.InnerException);
                return new Tuple<int, int, string>(-1, -1, "Unknown Error");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveData"></param>
        /// <returns></returns>
        /// Change to person data
        public bool UpdatePersonInformation(Employee saveData)
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
                        
                        return true;
                    }

                }               
            }
            //Catch all errors
            catch (Exception ex)
            {
                log.Error("");         
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
                        
                        cmd.Parameters.Clear();
                        MySqlParameter[] employeeParameters = new MySqlParameter[] 
                        {
                            new MySqlParameter { ParameterName = "emplID", Value = separationData.EmployeeID, MySqlDbType = MySqlDbType.Int32},
                            new MySqlParameter { ParameterName = "separationReasonCode", Value = separationData.SeparationCode, MySqlDbType = MySqlDbType.VarChar, Size = 3},
                            new MySqlParameter { ParameterName = "separationDate", Value = separationData.SeparationDate, MySqlDbType = MySqlDbType.Date},
                            new MySqlParameter { ParameterName = "persID", MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Output},
                            new MySqlParameter { ParameterName = "result", MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Output},
                            new MySqlParameter { ParameterName = "SQLExceptionWarning", MySqlDbType=MySqlDbType.VarChar, Size=4000, Direction = ParameterDirection.Output },
                        };

                        cmd.Parameters.AddRange(employeeParameters);
                       
                        cmd.ExecuteNonQuery();

                        return new Tuple<int, int, string>((int)cmd.Parameters["persID"].Value, (int)cmd.Parameters["result"].Value, cmd.Parameters["SQLExceptionWarning"].Value.ToString());
                    }

                }
            }
            catch (Exception ex)
            {
                log.Error("SaveSeparationInformation: " + separationData.EmployeeID + " - " + ex.Message + " - " + ex.InnerException);
                return new Tuple<int, int, string>(-1, -1, "Unknown Error");
            }                    
        }
    }
}