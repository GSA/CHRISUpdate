using HRUpdate.Process;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace HRUpdate
{
    static class Program
    {
        //Reference to logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //File paths from config file
        private static string hrFilePath = ConfigurationManager.AppSettings["HRFILE"].ToString();
        private static string separationFilePath = ConfigurationManager.AppSettings["SEPARATIONFILE"].ToString();        

        //Stopwatch objects
        private static Stopwatch timeForApp = new Stopwatch();
        private static Stopwatch timeForProcesses = new Stopwatch();

        /// <summary>
        /// Entrance into processsing the HR File
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //Start timer
            timeForApp.Start();

            //Log start of application
            log.Info("Application Started: " + DateTime.Now);

            //Output application start
            Console.WriteLine("Application Started: " + DateTime.Now);

            //Instantiate object that does processing
            ProcessData processData = new ProcessData();

            //Log action
            log.Info("Processing HR Links File:" + DateTime.Now);

            //Start timer
            timeForProcesses.Start();

            //Only process if the file is there
            if (File.Exists(hrFilePath))
                processData.ProcessHRFile(hrFilePath);

            //Only process if the file is there
            if (File.Exists(separationFilePath))
                processData.ProcessSeparationFile(separationFilePath);

            //Log action
            log.Info("Done Processing HR Links File:" + DateTime.Now);

            //Stop timer
            timeForProcesses.Stop();

#if DEBUG
            Console.WriteLine(string.Format("Processed Files in {0} milliseconds", timeForProcesses.ElapsedMilliseconds));
#endif
            //Log elapsed time for processing
            log.Info(string.Format("Processed Files in {0} milliseconds", timeForProcesses.ElapsedMilliseconds));

            //Stop second timer
            timeForApp.Stop();

            //Output total time
            Console.WriteLine(string.Format("Application Completed in {0} milliseconds", timeForApp.ElapsedMilliseconds));

            //Log total time
            log.Info(string.Format("Application Completed in {0} milliseconds", timeForApp.ElapsedMilliseconds));

            //Log application end
            log.Info("Application Done: " + DateTime.Now);

#if DEBUG
            //Wait for key press
            Console.ReadLine();
#endif
        }
    }
}

