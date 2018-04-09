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

        //Empty Contructor
        public SaveData(){}
        
        public Tuple<int, int, string, Employee> GetGCIMSRecord(string EmployeeID, string ssn, string lastName, string dateOfBirth)
        {
            try
            {
                using (conn)
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    using (cmd)
                    {
                        MySqlDataReader dr;

                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "HR_GetRecord";

                        cmd.Parameters.Clear();
                        MySqlParameter[] personParameters = new MySqlParameter[]
                        {
                            new MySqlParameter { ParameterName = "ssn", Value = ssn, MySqlDbType = MySqlDbType.VarChar, Size = 9},
                            new MySqlParameter { ParameterName = "lastName", Value = lastName, MySqlDbType = MySqlDbType.VarChar, Size = 60},
                            new MySqlParameter { ParameterName = "dateOfBirth", Value = dateOfBirth, MySqlDbType = MySqlDbType.VarChar, Size = 10},
                            new MySqlParameter { ParameterName = "emplID", MySqlDbType = MySqlDbType.VarChar, Size = 12 },
                            new MySqlParameter { ParameterName = "persID", MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Output},
                            new MySqlParameter { ParameterName = "result", MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Output},
                            new MySqlParameter { ParameterName = "SQLExceptionWarning", MySqlDbType=MySqlDbType.VarChar, Size=4000, Direction = ParameterDirection.Output },
                        };

                        cmd.Parameters.AddRange(personParameters);

                        Mapper.Initialize(cfg =>
                        {
                            cfg.AddDataReaderMapping();

                            cfg.CreateMap<Employee, Address>().ForMember(dest => dest.HomeAddress1, opt => opt.Ignore());
                            cfg.CreateMap<Employee, Person>().ForMember(dest => dest.SSN, opt => opt.Ignore());

                        });

                        //Mapper.AssertConfigurationIsValid();

                        MySqlDataReader gcimsData;

                        Employee gcimsRecord = new Employee();

                        gcimsData = cmd.ExecuteReader();

                        using (gcimsData)
                        {
                            if (gcimsData.HasRows)
                            {
                                gcimsRecord = LoadGCIMSData(gcimsData);
                            }
                        }                           
                                                 
                        return new Tuple<int, int, string, Employee>((int)cmd.Parameters["persID"].Value, (int)cmd.Parameters["result"].Value, cmd.Parameters["SQLExceptionWarning"].Value.ToString(), gcimsRecord);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetGCIMSRecord: " + EmployeeID + " - " + ex.Message + " - " + ex.InnerException);
                return new Tuple<int, int, string, Employee>(-1, -1, "Unknown Error", null);
            }
        }

        private Employee LoadGCIMSData(MySqlDataReader gcimsData)
        {            
            Employee employee = new Employee();

            Employee eTest = new Employee();

            Address address = new Address();
            Birth birth = new Birth();
            Detail detail;
            Emergency emergency = new Emergency();
            Investigation investigation;
            Person person = new Person();
            Position position;

            while (gcimsData.Read())
            {   
                address = Mapper.Map<IDataReader, Address>(gcimsData);
                birth = Mapper.Map<IDataReader, Birth>(gcimsData);
                //Mapper.Map<IDataReader, Detail>(gcimsData);
                emergency = Mapper.Map<IDataReader, Emergency>(gcimsData);
                //Mapper.Map<IDataReader, Investigation>(gcimsData);
                person = Mapper.Map<IDataReader, Person>(gcimsData);
                //position = Mapper.Map<IDataReader, Position>(gcimsData);
                eTest = Mapper.Map<IDataReader, Employee>(gcimsData);
            }


            employee.Birth = birth;
            employee.Address = address;
            employee.Emergency = emergency;
            employee.Person = person;

            return employee;
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