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

            //Call function to map file to csv
            //one = GetFileData<Person, PersonMapping>(chrisFile);
            //two = GetFileData<Person, PersonMapping>(chrisFile);

            CompareLogic compareLogic = new CompareLogic();
            compareLogic.Config.MaxDifferences = 500;

            hrData = GetFileData<Employee, EmployeeMapping>(chrisFile);
            gcimsData = GetFileData<Employee, EmployeeMapping>(chrisFile);

            var hrFilter = hrData.Where(w => w.Person.EmployeeID == "00004624");
            var gcimsFilter = gcimsData.Where(w => w.Person.EmployeeID == "00007172");

            // oneFilter = one.Where(w => w.ID == "00004624");
            //var twoFilter = one.Where(w => w.ID == "00007172");

            ComparisonResult result = compareLogic.Compare(hrFilter, gcimsFilter);

            if (!result.AreEqual)
                Console.WriteLine(result.DifferencesString);

            Console.ReadLine();
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
                List<Employee> employeeData;

                //Call function to map file to csv
                employeeData = GetFileData<Employee, EmployeeMapping>(hrFile);

                Tuple<int, int, string> personResults;

                //Start Processin the HR Data
                foreach (Employee employee in employeeData)
                {
                    personResults = save.GetGCIMSRecord(employee.Person.EmployeeID, employee.Person.SSN, employee.Person.LastName, employee.Birth.DateOfBirth?.ToString("yyyy-M-dd"));

                    Console.WriteLine(personResults.Item1);
                   
                    int personID = 0;
                    
                    //If person id > 0 meaning it found a person id
                    if (1 == 1) //(GetPersonID(SSN, out personID) > 0)
                    {
                        //Assign the Id to all the associated locations
                        //hrData.Employee.PersonID = personID;

                        //PersonID and Set Supervisor name (we are not able to map this as it's a combined field)
                        //Reason personID is apart of person is we can just pass the object to the save method
                        //hrData.Person.PersonID = personID;
                        //chrisData.Person.Supervisor = chrisData.Supervisor.LastNameSuffix + ", " + chrisData.Supervisor.FirstName + " " + chrisData.Supervisor.MiddleName;

                        //Save the data
                        //save.SaveCHRISInformation(hrData);

                        //Increment processed
                        processedUsers += 1;
                    }
                //If a certain percentage is met (the threshold) then we need to generate an error file. (Are we doing this, we are having a meeting about this stuff)
                    else
                    {                      
                        //Increment unprocessed
                        unprocessedUsers += 1;

                        //Log not found warning
                        //log.Warn("Not Found! " + hrData.Employee.EmployeeID + " " + hrData.Employee.FirstName + " " + hrData.Employee.LastName);
                    }

                    processedRecords += 1;
                }

                //Add log entries
                log.Info("CHRIS Records Processed: " + String.Format("{0:#,###0}", processedUsers));
                log.Info("CHRIS Users Not Processed: " + String.Format("{0:#,###0}", unprocessedUsers));
                log.Info("CHRIS Processed Records: " + String.Format("{0:#,###0}", processedRecords));                
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
                       
            processedRecords = 0;
            processedUsers = 0;
            unprocessedUsers = 0;
                       
            List<Separation> separationList;
            List<SeperationSummary> separationSuccessSummary = new List<SeperationSummary>();
            List<SeperationSummary> separationErrorSummary = new List<SeperationSummary>();

            //Call function that loads file and maps to csv
            separationList = GetFileData<Separation, CustomSeparationMap>(separationFile);
            
            try
            {
                //Iterate the data
                foreach (Separation separationData in separationList)
                {
                    Tuple<int, int, string> separationResults;

                    separationResults = save.SaveSeparationInformation(separationData);
                                        
                    if (separationResults.Item1 > 0)
                    {
                        var separationSuccess = separationList
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
                        var separationIssue = separationList
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

                //Log Separation Process
                log.Info("Separation Records Processed: " + String.Format("{0:#,###0}", processedUsers));
                log.Info("Separation Users Not Processed: " + String.Format("{0:#,###0}", unprocessedUsers));
                log.Info("Separation Processed Records: " + String.Format("{0:#,###0}", processedRecords));

                GenerateSeparationSummaryFiles(separationSuccessSummary, separationErrorSummary);
            }
            //Catch all errorsSeperationMapping
            catch (Exception ex)
            {
                //Log error
                log.Error("Process Separation Users Error:" + ex.Message + " " + ex.InnerException); 
            }
        }

        private void GenerateSeparationSummaryFiles(List<SeperationSummary> separationSuccessSummary, List<SeperationSummary> separationErrorSummary)
        {
            summaryFileGenerator.GenerateSummaryFile<SeperationSummary, SeperationMapping>(ConfigurationManager.AppSettings["SEPARATIONSUMMARYFILENAME"].ToString(), separationSuccessSummary);
            summaryFileGenerator.GenerateSummaryFile<SeperationSummary, SeperationMapping>(ConfigurationManager.AppSettings["SEPARATIONERRORSUMMARYFILENAME"].ToString(), separationErrorSummary);
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