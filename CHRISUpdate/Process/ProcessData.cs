using CsvHelper;
using CsvHelper.Configuration;
using HRUpdate.Mapping;
using HRUpdate.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;

namespace HRUpdate.Process
{    
    class ProcessData
    {
        //Reference to logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Class to work with CSV's
        private static CsvConfiguration config = new CsvConfiguration();

        int processedRecords = 0; //rolling count of records that were processed
        int processedUsers = 0;  //rolling count of users processed
        int unprocessedUsers = 0; //rolling count of unprocessed users

        readonly SaveData save = new SaveData();        

        //Constructor
        //Assigns defaults
        public ProcessData()
        {
            config.Delimiter = "~";
            config.HasHeaderRecord = false;
            config.WillThrowOnMissingField = false;
        }      

        /// <summary>
        /// Processes chris file
        /// </summary>
        /// <param name="chrisFile"></param>
        public void ProcessHRFile(string chrisFile)
        {
            //Log start of processing file
            log.Info("Processing HR Users");

            try
            {
                List<HR> hrList;

                //Call function to map file to csv
                hrList = GetFileData<HR, HRMapping>(chrisFile, config);

                //Start Processin the HR Data
                foreach (HR hrData in hrList)
                {
                    int personID = 0;

                    //Hash the ssn 
                    //byte[] SSN;
                    //SSN = u.HashSSN(hrData.SSN);

                    //If person id > 0 meaning it found a person id
                    if (1 == 1) //(GetPersonID(SSN, out personID) > 0)
                    {
                        //Assign the Id to all the associated locations
                        hrData.Employee.PersonID = personID;

                        //PersonID and Set Supervisor name (we are not able to map this as it's a combined field)
                        //Reason personID is apart of person is we can just pass the object to the save method
                        hrData.Person.PersonID = personID;
                        //chrisData.Person.Supervisor = chrisData.Supervisor.LastNameSuffix + ", " + chrisData.Supervisor.FirstName + " " + chrisData.Supervisor.MiddleName;

                        //Save the data
                        save.SaveCHRISInformation(hrData);

                        //Increment processed
                        processedUsers += 1;
                    }
                //If a certain percentage is met (the threshold) then we need to generate an error file. (Are we doing this, we are having a meeting about this stuff)
                    else
                    {                      
                        //Increment unprocessed
                        unprocessedUsers += 1;

                        //Log not found warning
                        log.Warn("Not Found! " + hrData.Employee.EmployeeID + " " + hrData.Employee.FirstName + " " + hrData.Employee.LastName);
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
            //Log function start
            log.Info("Processing Separation Users");            

            //Initialize counters
            processedRecords = 0;
            processedUsers = 0;
            unprocessedUsers = 0;

            //Initialize list to hold mapped csv data
            List<Separation> separationList;

            //Call function that loads file and maps to csv
            separationList = GetFileData<Separation, CustomSeparationMap>(separationFile, config);

            try
            {
                //Iterate the data
                foreach (Separation separationData in separationList)
                {
                    Tuple<int, int, string> separationResults;

                    separationResults = save.SaveSeparationInformation(separationData);

                    if (separationResults.Item1 > 0)
                    {
                        processedUsers += 1;
                    }
                    else
                    {
                        unprocessedUsers += 1;
                    }

                    processedRecords += 1;                    
                }

                //Log Separation Process
                log.Info("Separation Records Processed: " + String.Format("{0:#,###0}", processedUsers));
                log.Info("Separation Users Not Processed: " + String.Format("{0:#,###0}", unprocessedUsers));
                log.Info("Separation Processed Records: " + String.Format("{0:#,###0}", processedRecords));
            }
            //Catch all errors
            catch (Exception ex)
            {
                //Log error
                log.Error("Process Separation Users Error:" + ex.Message + " " + ex.InnerException); 
            }
        }   

        /// <summary>
        /// Takes a file and loads the data into the object type specified using the mapping
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <typeparam name="TMap"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="config"></param>
        /// <returns>A list of the type of objects specified</returns>
        private List<TClass> GetFileData<TClass, TMap>(string filePath, CsvConfiguration config)
            where TClass : class
            where TMap : CsvClassMap<TClass>
        {
            CsvParser csvParser = new CsvParser(new StreamReader(filePath), config);
            CsvReader csvReader = new CsvReader(csvParser);            

            csvReader.Configuration.RegisterClassMap<TMap>();

            return csvReader.GetRecords<TClass>().ToList();
        }
    }
}