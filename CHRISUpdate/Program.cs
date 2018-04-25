using AutoMapper;
using HRUpdate.Mapping;
using HRUpdate.Process;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

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

        private static IMapper saveMapper;      

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

            //Output application start
            Console.WriteLine("Application Started: " + DateTime.Now);

            CreateMaps();

            //Instantiate object that does processing
            ProcessData processData = new ProcessData(saveMapper);            

            //Log action
            log.Info("Processing HR Files:" + DateTime.Now);

            //HR File
            if (File.Exists(hrFilePath))
            {
                log.Info("Starting Processing HR File: " + DateTime.Now);

                timeForProcess.Start();
                processData.ProcessHRFile(hrFilePath);
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
                processData.ProcessSeparationFile(separationFilePath);
                timeForProcess.Stop();

                log.Info("Done Processing Separation File: " + DateTime.Now);
                log.Info("Separation File Processing Time: " + timeForProcess.ElapsedMilliseconds);
            }
            else
            {
                log.Error("Separation File Not Found");
            }

            log.Info("Done Processing HR Links File(s):" + DateTime.Now);

            processData.SendSummaryEMail();

            //Stop second timer
            timeForApp.Stop();

            //Output total time
            Console.WriteLine(string.Format("Application Completed in {0} milliseconds", timeForApp.ElapsedMilliseconds));

            //Log total time
            log.Info(string.Format("Application Completed in {0} milliseconds", timeForApp.ElapsedMilliseconds));

            //Log application end
            log.Info("Application Done: " + DateTime.Now);

            //Output application start
            Console.WriteLine("Application Ended: " + DateTime.Now);

#if DEBUG
            //Wait for key press
            Console.ReadLine();
#endif
        }

        private static void CreateMaps()
        {            
            map.CreateSaveConfig();
            saveMapper = map.CreateSaveMapping();
        }
    }
}