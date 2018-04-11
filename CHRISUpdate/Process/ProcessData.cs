using CsvHelper;
using CsvHelper.Configuration;
using HRLinks.Mapping;
using HRUpdate.Mapping;
using HRUpdate.Models;
using HRUpdate.Utilities;
using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace HRUpdate.Process
{
    class ProcessData
    {
        //Reference to logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly SummaryFileGenerator summaryFileGenerator = new SummaryFileGenerator();
        private readonly SaveData save = new SaveData();

        int processedRecords = 0; //rolling count of records that were processed
        int processedUsers = 0;  //rolling count of users processed
        int unprocessedUsers = 0; //rolling count of unprocessed users

        //Constructor
        public ProcessData() { }

        public void CompareObject(string chrisFile)
        {            
            List<Employee> hrData;
            List<Employee> gcimsData;           

            CompareLogic compareLogic = new CompareLogic();
            compareLogic.Config.MaxDifferences = 500;

            hrData = GetFileData<Employee, EmployeeMapping>(chrisFile);
            gcimsData = GetFileData<Employee, EmployeeMapping>(chrisFile);

            var hrFilter = hrData.Where(w => w.Person.EmployeeID == "00004624");
            var gcimsFilter = gcimsData.Where(w => w.Person.EmployeeID == "00007172");            

            ComparisonResult result = compareLogic.Compare(hrFilter, gcimsFilter);

            if (!result.AreEqual)
                Console.WriteLine(result.DifferencesString);

            Console.ReadLine();
        }

        private bool AreEqualGCIMSToHR(Employee GCIMSData, Employee HRData)
        {
            CompareLogic compareLogic = new CompareLogic();
            compareLogic.Config.MembersToIgnore.Add("Person.SSN");

            ComparisonResult result = compareLogic.Compare(GCIMSData, HRData);

            return result.AreEqual;

            //if (!result.AreEqual)
            //    Console.WriteLine(result.DifferencesString);
        }

        /// <summary>
        /// Get HR Data
        /// Loop HR Data
        /// Get GCIMS Record
        /// Update GCIMS Record
        /// </summary>
        /// <param name="hrFile"></param>
        public void ProcessHRFile(string hrFile)
        {
            log.Info("Processing HR Users");

            try
            {
                processedRecords = 0;
                processedUsers = 0;
                unprocessedUsers = 0;

                List<Employee> usersToProcess;
                List<ProcessedSummary> usersProcessedSuccessfullySummary = new List<ProcessedSummary>();
                List<ProcessedSummary> usersProcessedErrorSummary = new List<ProcessedSummary>();

                //Call function to map file to csv
                usersToProcess = GetFileData<Employee, EmployeeMapping>(hrFile);

                Tuple<int, int, string, Employee> personResults;

                //Start Processin the HR Data
                foreach (Employee employeeData in usersToProcess)
                {
                    personResults = save.GetGCIMSRecord(employeeData.Person.EmployeeID, employeeData.Person.SSN, employeeData.Person.LastName, employeeData.Birth.DateOfBirth?.ToString("yyyy-M-dd"));
                    
                    int personID = personResults.Item1;                    
                    
                    if (personID > 0 && !AreEqualGCIMSToHR(personResults.Item4, employeeData))
                    {
                        Console.WriteLine("Update Record");

                        //SaveData

                        var processedUserSuccess = usersToProcess
                             .Where(w => w.Person.EmployeeID == employeeData.Person.EmployeeID)
                             .Select
                                 (
                                     s =>
                                         new ProcessedSummary
                                         {
                                             ID = personResults.Item1,
                                             FirstName = s.Person.FirstName,
                                             MiddleName = s.Person.MiddleName,
                                             LastName = s.Person.LastName,
                                             Action = "Success"
                                         }
                                 ).ToList();

                        usersProcessedSuccessfullySummary.AddRange(processedUserSuccess);

                        processedUsers += 1;
                    }
                    else
                    {
                        var proccessedUserIssue = usersToProcess
                            .Where(w => w.Person.EmployeeID == employeeData.Person.EmployeeID)
                            .Select
                                 (
                                     s =>
                                         new ProcessedSummary
                                         {
                                             ID = personResults.Item1,
                                             FirstName = s.Person.FirstName,
                                             MiddleName = s.Person.MiddleName,
                                             LastName = s.Person.LastName,
                                             Action = "Unknown"
                                         }
                                 ).ToList();


                        usersProcessedErrorSummary.AddRange(proccessedUserIssue);

                        unprocessedUsers += 1;
                    }

                    processedRecords += 1;
                }

                //Add log entries
                log.Info("GcIMS Records Processed: " + String.Format("{0:#,###0}", processedUsers));
                log.Info("CHRIS Users Not Processed: " + String.Format("{0:#,###0}", unprocessedUsers));
                log.Info("CHRIS Processed Records: " + String.Format("{0:#,###0}", processedRecords));

                GenerateUsersProccessedSummaryFiles(usersProcessedSuccessfullySummary, usersProcessedErrorSummary);
            }
            //Catch all errors
            catch (Exception ex)
            {
                log.Error("Process HR Users Error:" + ex.Message + " " + ex.InnerException + " " + ex.StackTrace);                
            }
        }

        /// <summary>
        /// Process separation file
        /// </summary>
        /// <param name="separationFile"></param>
        public void ProcessSeparationFile(string separationFile)
        {
            log.Info("Processing Separation Users");            
            
                        
            try
            {
                processedRecords = 0;
                processedUsers = 0;
                unprocessedUsers = 0;

                List<Separation> separationUsersToProcess;
                List<SeperationSummary> separationSuccessSummary = new List<SeperationSummary>();
                List<SeperationSummary> separationErrorSummary = new List<SeperationSummary>();

                separationUsersToProcess = GetFileData<Separation, CustomSeparationMap>(separationFile);

                Tuple<int, int, string> separationResults;

                foreach (Separation separationData in separationUsersToProcess)
                {
                    separationResults = save.SaveSeparationInformation(separationData);
                                        
                    if (separationResults.Item1 > 0)
                    {
                        var separationSuccess = separationUsersToProcess
                             .Where(w => w.EmployeeID == separationData.EmployeeID)
                             .Select
                                 (
                                     s =>
                                         new SeperationSummary
                                         {
                                             GCIMSID = separationResults.Item1,
                                             EmployeeID = s.EmployeeID,
                                             SeparationCode = s.SeparationCode,
                                             Action = "Success"
                                         }
                                 ).ToList();

                        separationSuccessSummary.AddRange(separationSuccess);

                        processedUsers += 1;
                    }
                    else
                    {
                        var separationIssue = separationUsersToProcess
                            .Where(w => w.EmployeeID == separationData.EmployeeID)
                            .Select
                                (
                                    s =>
                                        new SeperationSummary
                                        {
                                            GCIMSID = separationResults.Item1,
                                            EmployeeID = s.EmployeeID,
                                            SeparationCode = s.SeparationCode,
                                            Action = "Unknown"
                                        }
                                ).ToList();


                        separationErrorSummary.AddRange(separationIssue);

                        unprocessedUsers += 1;
                    }

                    processedRecords += 1;                    
                }
                
                log.Info("Separation Records Processed: " + String.Format("{0:#,###0}", processedUsers));
                log.Info("Separation Users Not Processed: " + String.Format("{0:#,###0}", unprocessedUsers));
                log.Info("Separation Processed Records: " + String.Format("{0:#,###0}", processedRecords));

                GenerateSeparationSummaryFiles(separationSuccessSummary, separationErrorSummary);
            }
            catch (Exception ex)
            {   
                log.Error("Process Separation Users Error:" + ex.Message + " " + ex.InnerException); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="separationSuccessSummary"></param>
        /// <param name="separationErrorSummary"></param>
        private void GenerateSeparationSummaryFiles(List<SeperationSummary> separationSuccessSummary, List<SeperationSummary> separationErrorSummary)
        {
            summaryFileGenerator.GenerateSummaryFile<SeperationSummary, SeperationMapping>(ConfigurationManager.AppSettings["SEPARATIONSUMMARYFILENAME"].ToString(), separationSuccessSummary);
            summaryFileGenerator.GenerateSummaryFile<SeperationSummary, SeperationMapping>(ConfigurationManager.AppSettings["SEPARATIONERRORSUMMARYFILENAME"].ToString(), separationErrorSummary);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processedSuccessSummary"></param>
        /// <param name="processedErrorSummary"></param>
        private void GenerateUsersProccessedSummaryFiles(List<ProcessedSummary> usersProcessedSuccessSummary, List<ProcessedSummary> usersProcessedErrorSummary)
        {
            summaryFileGenerator.GenerateSummaryFile<ProcessedSummary, ProcessedSummaryMappng>(ConfigurationManager.AppSettings["SUCCESSSUMMARYFILENAME"].ToString(), usersProcessedSuccessSummary);
            summaryFileGenerator.GenerateSummaryFile<ProcessedSummary, ProcessedSummaryMappng>(ConfigurationManager.AppSettings["ERRORSUMMARYFILENAME"].ToString(), usersProcessedErrorSummary);
        }

        /// <summary>
        /// Takes a file and loads the data into the object type specified using the mapping
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <typeparam name="TMap"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="config"></param>
        /// <returns>A list of the type of objects specified</returns>
        private List<TClass> GetFileData<TClass, TMap>(string filePath)
            where TClass : class
            where TMap : ClassMap<TClass>
        {
            CsvParser csvParser = new CsvParser(new StreamReader(filePath));
            CsvReader csvReader = new CsvReader(csvParser);

            csvReader.Configuration.Delimiter = "~";
            csvReader.Configuration.HasHeaderRecord = false;

            csvReader.Configuration.RegisterClassMap<TMap>();

            return csvReader.GetRecords<TClass>().ToList();
        }
    }
}