using AutoMapper;
using HRUpdate.Data;
using HRUpdate.Lookups;
using HRUpdate.Mapping;
using HRUpdate.Models;
using HRUpdate.Process;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using HRUpdate.Utilities;
using System.Collections.Generic;

namespace HRUpdate
{
    internal static class Program
    {
        //Reference to logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //File paths from config file
        private static string hrFilePath = ConfigurationManager.AppSettings["HRFILE"].ToString();

        private static string separationFilePath = ConfigurationManager.AppSettings["SEPARATIONFILE"].ToString();

        //Stopwatch objects
        private static Stopwatch timeForApp = new Stopwatch();

        private static Stopwatch timeForProcess = new Stopwatch();

        private static HRMapper map = new HRMapper();

        private static IMapper dataMapper;

        private static EMailData emailData = new EMailData();

        /// <summary>
        /// Entrance into processing the HR File
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            //Start timer
            timeForApp.Start();

            //Log start of application
            log.Info("Application Started: " + DateTime.Now);

            CreateMaps();

            Lookup lookups = createLookups();

            ProcessHR processHR = new ProcessHR(dataMapper, ref emailData, lookups);
            ProcessSeparation processSeparation = new ProcessSeparation(ref emailData);
            SendSummary sendSummary = new SendSummary(ref emailData);
            Dictionary<string, object> objects = new Dictionary<string, object>();

            //Log action
            log.Info("Processing HR Files:" + DateTime.Now);

            //HR File
            if (File.Exists(hrFilePath))
            {
                log.Info("Starting Processing HR File: " + DateTime.Now);

                timeForProcess.Start();
                objects = processHR.ProcessHRFile(hrFilePath);
                timeForProcess.Stop();

                log.Info("Done Processing HR File: " + DateTime.Now);
                log.Info("HR File Processing Time: " + timeForProcess.ElapsedMilliseconds);
            }
            else
            {
                log.Error("HR Links File Not Found");
            }

            //Separation File
            if (File.Exists(separationFilePath))
            {
                log.Info("Starting Processing Separation File: " + DateTime.Now);

                timeForProcess.Start();
                processSeparation.ProcessSeparationFile(separationFilePath);
                timeForProcess.Stop();

                log.Info("Done Processing Separation File: " + DateTime.Now);
                log.Info("Separation File Processing Time: " + timeForProcess.ElapsedMilliseconds);
            }
            else
            {
                log.Error("Separation File Not Found");
            }

            log.Info("Done Processing HR Links File(s):" + DateTime.Now);

            log.Info("Sending Summary File");
            sendSummary.SendSummaryEMail();
            log.Info("Summary file sent");

            //Stop second timer
            timeForApp.Stop();

            //Delete files on successful run
            Delete delete = new Delete();
            delete.DeleteProcessFiles(hrFilePath, separationFilePath, objects);

            //Log total time
            log.Info(string.Format("Application Completed in {0} milliseconds", timeForApp.ElapsedMilliseconds));
            
            //Log application end
            log.Info("Application Done: " + DateTime.Now);
        }

        

        private static void CreateMaps()
        {
            map.CreateDataConfig();
            dataMapper = map.CreateDataMapping();
        }

        private static Lookup createLookups()
        {
            Lookup lookups;
            HRMapper hrmap = new HRMapper();
            IMapper lookupMapper;

            hrmap.CreateLookupConfig();

            lookupMapper = hrmap.CreateLookupMapping();

            LoadLookupData loadLookupData = new LoadLookupData(lookupMapper);

            lookups = loadLookupData.GetEmployeeLookupData();

            return lookups;
        }
    }
}