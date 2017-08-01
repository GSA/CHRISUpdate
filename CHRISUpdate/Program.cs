using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using CHRISUpdate.Process;

namespace CHRISUpdate
{
    class Program
    {
        //Reference to logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //File paths from config file
        private static string chrisFilePath = ConfigurationManager.AppSettings["CHRISFILENAME"].ToString(); //AppDomain.CurrentDomain.BaseDirectory + 
        private static string separationFilePath = ConfigurationManager.AppSettings["SEPARATIONFILENAME"].ToString(); //AppDomain.CurrentDomain.BaseDirectory + 
        private static string organizationFilePath = ConfigurationManager.AppSettings["ORGFILENAME"].ToString(); //AppDomain.CurrentDomain.BaseDirectory + 

        //Stopwatch objects
        private static Stopwatch stopWatch = new Stopwatch();
        private static Stopwatch timeForProcesses = new Stopwatch();

        /// <summary>
        /// Main 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //Start timer
            stopWatch.Start();

            //Log start of application
            log.Info("Application Started");

            //Output application start
            Console.WriteLine("Application Started: " + DateTime.Now);

            //Instantiate object that does processing
            ProcessData processData = new ProcessData();

            //Log action
            log.Info("Processing CHRIS File");

            //Call function that starts processing file
            timeForProcesses.Start();

            //Only process if the file is there
            if (File.Exists(chrisFilePath))
                processData.ProcessCHRISFile(chrisFilePath);

            //Only process if the file is there
            if (File.Exists(separationFilePath))
                processData.ProcessSeparationFile(separationFilePath);

            //Process if file exists
            if (File.Exists(organizationFilePath))
                processData.ProcessOrganizationFile(organizationFilePath);

            //Stop timer
            timeForProcesses.Stop();

#if DEBUG
            Console.WriteLine(string.Format("Processed Files in {0} milliseconds", timeForProcesses.ElapsedMilliseconds));
#endif
            //Log elapsed time for processing
            log.Info(string.Format("Processed Files in {0} milliseconds", timeForProcesses.ElapsedMilliseconds));

            //Stop second timer
            stopWatch.Stop();

            //Output total time
            Console.WriteLine(string.Format("Application Completed in {0} milliseconds", stopWatch.ElapsedMilliseconds));

            //Log total time
            log.Info(string.Format("Application Completed in {0} milliseconds", stopWatch.ElapsedMilliseconds));

            //Log application end
            log.Info("Application Done");

#if DEBUG
            //Wait for key press
            Console.ReadLine();
#endif
        }
    }
}

