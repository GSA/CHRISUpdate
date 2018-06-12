using AutoMapper;
using HRUpdate.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace HRUpdate.Data
{
    internal class RetrieveData
    {
        //Reference to logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IMapper retrieveMapper;

        public RetrieveData(IMapper mapper)
        {
            retrieveMapper = mapper;

            retrieveMapper.ConfigurationProvider.CompileMappings();
        }

        public List<Employee> AllGCIMSData()
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GCIMS"].ToString());

                MySqlCommand cmd = new MySqlCommand();

                List<Employee> allGCIMSData = new List<Employee>();

                using (conn)
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    using (cmd)
                    {
                        MySqlDataReader gcimsData;

                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "HR_GetAllRecords";
                        cmd.Parameters.Clear();

                        gcimsData = cmd.ExecuteReader();

                        using (gcimsData)
                        {
                            if (gcimsData.HasRows)
                            {
                                allGCIMSData = MapAllGCIMSData(gcimsData);
                            }
                        }
                    }
                }

                return allGCIMSData;
            }
            catch (Exception ex)
            {
                log.Error("GetGCIMSRecord: " + " - " + ex.Message + " - " + ex.InnerException);
                return new List<Employee>();
            }
        }

        private List<Employee> MapAllGCIMSData(MySqlDataReader gcimsData)
        {
            List<Employee> allRecords = new List<Employee>();

            while (gcimsData.Read())
            {
                Employee employee = new Employee();

                employee.Address = retrieveMapper.Map<IDataReader, Address>(gcimsData);
                employee.Building = retrieveMapper.Map<IDataReader, Building>(gcimsData);
                employee.Birth = retrieveMapper.Map<IDataReader, Birth>(gcimsData);
                employee.Emergency = retrieveMapper.Map<IDataReader, Emergency>(gcimsData);
                employee.Investigation = retrieveMapper.Map<IDataReader, Investigation>(gcimsData);
                employee.Person = retrieveMapper.Map<IDataReader, Person>(gcimsData);
                employee.Phone = retrieveMapper.Map<IDataReader, Phone>(gcimsData);
                employee.Position = retrieveMapper.Map<IDataReader, Position>(gcimsData); //Need to fix SupervisorID

                allRecords.Add(employee);
            }

            return allRecords;
        }
    }
}