using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using CHRISUpdate.Process;

namespace CHRISUpdate
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string chrisFilePath = ConfigurationManager.AppSettings["CHRISFILENAME"].ToString(); //AppDomain.CurrentDomain.BaseDirectory + 
        private static string separationFilePath = ConfigurationManager.AppSettings["SEPARATIONFILENAME"].ToString(); //AppDomain.CurrentDomain.BaseDirectory + 
        private static string organizationFilePath = ConfigurationManager.AppSettings["ORGFILENAME"].ToString(); //AppDomain.CurrentDomain.BaseDirectory + 

        private static Stopwatch stopWatch = new Stopwatch();
        private static Stopwatch timeForProcesses = new Stopwatch();

        static void Main(string[] args)
        {
            stopWatch.Start();

            log.Info("Application Started");

            Console.WriteLine("Application Started: " + DateTime.Now);

            //if (File.Exists(chrisFilePath))
            //{
                ProcessData processData = new ProcessData();

                log.Info("Processing CHRIS File");

                timeForProcesses.Start();

                //Only process if the file is there
                if (File.Exists(chrisFilePath))
                    processData.ProcessCHRISFile(chrisFilePath);

                //Only process if the file is there
                if (File.Exists(separationFilePath))
                    processData.ProcessSeparationFile(separationFilePath);

                if (File.Exists(organizationFilePath))
                    processData.ProcessOrganizationFile(organizationFilePath);

                timeForProcesses.Stop();

#if DEBUG
                Console.WriteLine(string.Format("Processed Files in {0} milliseoncds", timeForProcesses.ElapsedMilliseconds));
#endif
                log.Info(string.Format("Processed Files in {0} milliseconds", timeForProcesses.ElapsedMilliseconds));
            //}
            //else
            //{
            //    log.Warn("File Not Found!");
            //}

            stopWatch.Stop();

            Console.WriteLine(string.Format("Application Completed in {0} milliseconds", stopWatch.ElapsedMilliseconds));

            log.Info(string.Format("Application Completed in {0} milliseconds", stopWatch.ElapsedMilliseconds));

            log.Info("Application Done");

#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}

