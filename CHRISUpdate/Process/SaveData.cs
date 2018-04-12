using AutoMapper;
using AutoMapper.Data;
using HRUpdate.Models;
using HRUpdate.Mapping;
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

        //private readonly HRLinksMapper map = new HRLinksMapper();
        //private readonly IMapper mapper;

        //Empty Contructor
        public SaveData()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddDataReaderMapping();
                cfg.CreateMap<Employee, Person>().ForMember(dest => dest.SSN, opt => opt.Ignore());
            });
        }          

        public Tuple<int, int, string, string, Employee> GetGCIMSRecord(string EmployeeID, byte[] ssn, string lastName, string dateOfBirth)
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
                            new MySqlParameter { ParameterName = "ssn", Value = ssn, MySqlDbType = MySqlDbType.VarBinary, Size = 32},
                            new MySqlParameter { ParameterName = "lastName", Value = lastName, MySqlDbType = MySqlDbType.VarChar, Size = 60},
                            new MySqlParameter { ParameterName = "dateOfBirth", Value = dateOfBirth, MySqlDbType = MySqlDbType.VarChar, Size = 10},
                            new MySqlParameter { ParameterName = "emplID", MySqlDbType = MySqlDbType.VarChar, Size = 12 },
                            new MySqlParameter { ParameterName = "persID", MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Output},
                            new MySqlParameter { ParameterName = "result", MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Output},
                            new MySqlParameter { ParameterName = "actionMsg", MySqlDbType = MySqlDbType.VarChar, Size = 50, Direction = ParameterDirection.Output },
                            new MySqlParameter { ParameterName = "SQLExceptionWarning", MySqlDbType=MySqlDbType.VarChar, Size=4000, Direction = ParameterDirection.Output },
                        };

                        cmd.Parameters.AddRange(personParameters);
                        
                        MySqlDataReader gcimsData;

                        Employee gcimsRecord = new Employee();

                        gcimsData = cmd.ExecuteReader();

                        using (gcimsData)
                        {
                            if (gcimsData.HasRows)
                            {
                                gcimsRecord = MapGCIMSData(gcimsData);
                            }
                        }                           
                                                 
                        return new Tuple<int, int, string, string, Employee>((int)cmd.Parameters["persID"].Value, (int)cmd.Parameters["result"].Value, cmd.Parameters["actionMsg"].Value.ToString(), cmd.Parameters["SQLExceptionWarning"].Value.ToString(), gcimsRecord);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetGCIMSRecord: " + EmployeeID + " - " + ex.Message + " - " + ex.InnerException);
                return new Tuple<int, int, string, string, Employee>(-1, -1, ex.Message.ToString(), ex.InnerException.ToString(), null);
            }
        }

        private Employee MapGCIMSData(MySqlDataReader gcimsData)
        {            
            Employee employee = new Employee();            

            while (gcimsData.Read())
            {                
                employee.Address = Mapper.Map<IDataReader, Address>(gcimsData);
                employee.Birth = Mapper.Map<IDataReader, Birth>(gcimsData);
                employee.Emergency = Mapper.Map<IDataReader, Emergency>(gcimsData);
                employee.Investigaton = Mapper.Map<IDataReader, Investigation>(gcimsData);
                employee.Person = Mapper.Map<IDataReader, Person>(gcimsData);
                //employee.Position = Mapper.Map<IDataReader, Position>(gcimsData); //Need to fix SupervisorID          
            }

            return employee;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveData"></param>
        /// <returns></returns>
        /// Change to person data
        public bool UpdatePersonInformation(Employee hrData)
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

                        cmd.Parameters.Clear();

                        MySqlParameter[] personParameters = new MySqlParameter[]
                        {
                            new MySqlParameter { ParameterName = "emplID", Value = hrData.Person.EmployeeID, MySqlDbType = MySqlDbType.VarChar, Size = 11},
                        };

                        cmd.Parameters.AddRange(personParameters);

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
        public Tuple<int, int, string, string>SaveSeparationInformation(Separation separationData)
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
                return new Tuple<int, int, string, string>(-1, -1, "Unknown Error", "Uknown SQL Exception Warning");
            }                    
        }
    }
}