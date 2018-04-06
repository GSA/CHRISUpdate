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
                //Log error and return false for failure while saving
                //log.Warn("[SaveCHRISInformation] - Unable to save " + saveData.ChrisID + " - " + ex.Message + " - " + ex.InnerException);
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
    }
}