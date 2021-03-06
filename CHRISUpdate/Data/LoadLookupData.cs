﻿using AutoMapper;
using HRUpdate.Lookups;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace HRUpdate.Data
{
    internal class LoadLookupData
    {
        //Reference to logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Set up connection
        private readonly MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GCIMS"].ToString());

        private readonly MySqlCommand cmd = new MySqlCommand();

        private readonly IMapper lookupMapper;

        public LoadLookupData(IMapper mapper)
        {
            lookupMapper = mapper;

            lookupMapper.ConfigurationProvider.CompileMappings();
        }

        public Lookup GetEmployeeLookupData()
        {
            Lookup lookups = new Lookup();

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
                        cmd.CommandText = "HR_Get_Employee_Lookups";

                        MySqlDataReader lookupData = cmd.ExecuteReader();

                        using (lookupData)
                        {
                            if (lookupData.HasRows)
                                lookups = MapEmployeeLookupData(lookupData);
                        }
                    }
                }

                return lookups;
            }
            catch (Exception ex)
            {
                log.Error("Something went wrong" + " - " + ex.Message + " - " + ex.InnerException);

                return lookups;
            }
        }

        public Lookup GetSeparationLookupData()
        {
            Lookup lookups = new Lookup();

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
                        cmd.CommandText = "HR_Get_Separation_Lookup";

                        MySqlDataReader lookupData;

                        lookupData = cmd.ExecuteReader();

                        using (lookupData)
                        {
                            if (lookupData.HasRows)
                                lookups = MapSeparationLookupData(lookupData);
                        }
                    }
                }

                return lookups;
            }
            catch (Exception ex)
            {
                log.Error("Something went wrong" + " - " + ex.Message + " - " + ex.InnerException);

                return lookups;
            }
        }

        private Lookup MapEmployeeLookupData(MySqlDataReader lookupData)
        {
            Lookup lookup = new Lookup();

            //lookup_investigation
            lookup.investigationLookup = lookupMapper.Map<IDataReader, List<InvestigationLookup>>(lookupData);

            //lookup_country
            lookupData.NextResult();
            lookup.countryLookup = lookupMapper.Map<IDataReader, List<CountryLookup>>(lookupData);

            //lookup_state
            lookupData.NextResult();
            lookup.stateLookup = lookupMapper.Map<IDataReader, List<StateLookup>>(lookupData);

            //lookup_region
            lookupData.NextResult();
            lookup.regionLookup = lookupMapper.Map<IDataReader, List<RegionLookup>>(lookupData);

            //lookup_building
            lookupData.NextResult();
            lookup.BuildingLookup = lookupMapper.Map<IDataReader, List<BuildingLookup>>(lookupData);

            return lookup;
        }

        private Lookup MapSeparationLookupData(MySqlDataReader lookupData)
        {
            Lookup lookup = new Lookup();         
          
            lookup.separationLookup = lookupMapper.Map<IDataReader, List<SeparationLookup>>(lookupData);

            return lookup;
        }
    }
}