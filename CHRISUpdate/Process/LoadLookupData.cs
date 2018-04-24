using MySql.Data.MySqlClient;
using System.Configuration;
using HRUpdate.Lookups;
using System.Collections.Generic;
using System.Data;
using System;
using AutoMapper;
using AutoMapper.Data;

namespace HRUpdate.Process
{
    internal class LoadLookupData
    {
        //Reference to logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Set up connection
        private readonly MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GCIMS"].ToString());

        private readonly MySqlCommand cmd = new MySqlCommand();

        public LoadLookupData()
        {
            //Mapper.Initialize(cfg =>
            //{
            //    cfg.AddDataReaderMapping();
            //    cfg.AllowNullCollections = true;

            //    //cfg.CreateMap<Investigation_Lookup, Lookup>()
            //    //.ForMember(dest => dest.investigation_lookup, opt => opt.MapFrom(src => src.Tier))
            //    //.ForMember(dest => dest.investigation_lookup, opt => opt.MapFrom(src => src.Code));
            //});
        }

        public Lookup GetLookupData()
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
                        cmd.CommandText = "HR_Get_Lookups";

                        MySqlDataReader lookupData;

                        lookupData = cmd.ExecuteReader();

                        using (lookupData)
                        {
                            if (lookupData.HasRows)
                                lookups = MapLookupData(lookupData);
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

        private Lookup MapLookupData(MySqlDataReader lookupData)
        {
            Lookup lookup = new Lookup();

            while (lookupData.Read())
            {
                lookup.investigation_lookup = Mapper.Map<IDataReader, List<Investigation_Lookup>>(lookupData);
            }

            return lookup;
        }
    }
}