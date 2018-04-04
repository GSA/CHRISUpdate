using HRUpdate.Mapping;
using HRUpdate.Models;
using CsvHelper;
using CsvHelper.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace HRUpdate.Process
{
    //private static Stopwatch timeForProcesses = new Stopwatch();
    class ProcessData
    {
        //Reference to logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Class to work with CSV's
        private static CsvConfiguration config = new CsvConfiguration();

        //Set up database connections
        private readonly MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GCIMS"].ToString());
        private readonly MySqlCommand cmd = new MySqlCommand();
        private readonly MySqlConnection connHR = new MySqlConnection(ConfigurationManager.ConnectionStrings["HR"].ToString());
        private readonly MySqlCommand cmdHR = new MySqlCommand();

        //Class variables
        private static List<HR> chrisList = new List<HR>();
        private static List<HRData> chrisListData = new List<HRData>();

        int processedRecords = 0; //rolling count of records that were processed
        int processedUsers = 0;  //rolling count of users processed
        int unprocessedUsers = 0; //rolling count of unprocessed users

        readonly SaveData save = new SaveData();

        //Need better naming namespace and convention here
        readonly Utilities.Utilities u = new Utilities.Utilities();

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
        public void ProcessCHRISFile(string chrisFile)
        {
            //Log start of processing file
            log.Info("Processing HR Users");

            try
            {
                //Call function to map file to csv
                chrisList = GetFileData<HR, HRMapping>(chrisFile, config);

                //string json = JsonConvert.SerializeObject(chrisList.Where(w => w.EmployeeNumber == "00001575"), Formatting.Indented);
                
                //Yale
                //string json = JsonConvert.SerializeObject(chrisList.Where(e => new[] { "00004624", "00001604", "00002058", "00009957", "00009413" }.Contains(e.EmployeeNumber)), Formatting.Indented);

                //Jason
                //string json = JsonConvert.SerializeObject(chrisList.Where(e => new[] { "00008197", "00005573", "00005574", "00005575", "00005576" }.Contains(e.EmployeeNumber)), Formatting.Indented);

                foreach (HR hrData in chrisList) // Loop through List with foreach - Chris chrisData in chrisList
                {
                    int personID = 0;

                    //Hash the ssn 
                    byte[] SSN;
                    SSN = u.HashSSN(hrData.SSN);

                    //If person id > 0 meaning it found a person id
                    if (GetPersonID(SSN, out personID) > 0)
                    {
                        //Assign the Id to all the associated locations
                        hrData.Employee.PersonID = personID;

                        //PersonID and Set Supervisor name (we are not able to map this as it's a combined field)
                        //Reason personID is apart of person is we can just pass the object to the save method
                        hrData.Person.PersonID = personID;
                        //chrisData.Person.Supervisor = chrisData.Supervisor.LastNameSuffix + ", " + chrisData.Supervisor.FirstName + " " + chrisData.Supervisor.MiddleName;

                        //Save the data
                        save.SaveCHRISInformation(hrData, connHR);

                        //Increment processed
                        processedUsers += 1;
                    }
                //If a certain percentage is met (the threshold) then we need to generate an error file. (Are we doing this, we are having a meeting about this stuff)
                    else
                    {
                    //For now we are going to skip a person if they are not found in GCIMS.
                    //We are thinking this might be the source to actually add a new person to GCIMS.
                    //We need to talk to operations about this issue.
                    //log.Warn("Unable to process user");

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

                //Close connection
                connHR.Close();
                conn.Close();
            }
            //Catch all errors
            catch (Exception ex)
            {
                log.Error("Process CHRIS Users Error:" + ex.Message + " " + ex.InnerException + " " + ex.StackTrace);                
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

            //If we can't open the DB just crash here no point going forward (just making sure we have a connection)
            //if (connHR.State == ConnectionState.Closed)
            //{
            //    connHR.Open();
            //    cmdHR.Connection = connHR;
            //}

            //Initialize counters
            processedRecords = 0; //rolling count of records that were processed
            processedUsers = 0;  //rolling count of users processed
            unprocessedUsers = 0; //rolling count of unprocessed users

            //Initialize list to hold mapped csv data
            List<Separation> separationList; // = new List<Separation>();

            //Call function that loads file and maps to csv
            separationList = GetFileData<Separation, CustomSeparationMap>(separationFile, config);

            try
            {
                //Iterate through all data
                foreach (Separation separationData in separationList)
                {
                    //If employee exists based on employee id
                    if (DoesEmployeeExist(separationData.ChrisID)) //EmployeeID = Chris ID
                    {
                        //Save data
                        save.SaveSeparationInformation(separationData, connHR);

                        //Increment
                        processedUsers += 1;
                    }
                    //If a certain percentage is met (the threshold) then we need to generate an error file. (Are we doing this, we are having a meeting about this stuff)
                    else
                    {
                        //For now we are going to skip a person if they are not found in GCIMS.
                        //We are thinking this might be the source to actually add a new person to GCIMS.
                        //We need to talk to operations about this issue.
                        //log.Warn("Unable to process user");
                        unprocessedUsers += 1;
                        log.Warn("Not Found! " + separationData.ChrisID);
                    }

                    //Moved here on 9/15 was in the wrong location and was reporting 1 always
                    processedRecords += 1;
                }

                //Add to log
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
        /// Process organization file
        /// </summary>
        /// <param name="organizationFile"></param>
        public void ProcessOrganizationFile(string organizationFile)
        {
            //Log start of function
            log.Info("Processing Separation Users");

            //If we can't open the DB just crash here no point going forward (just making sure we have a connection)
            //if (conn.State == ConnectionState.Closed)
            //{
            //    conn.Open();
            //    cmd.Connection = conn;
            //}

            //Declare counters
            processedRecords = 0; //rolling count of records that were processed
            processedUsers = 0;  //rolling count of users processed
            unprocessedUsers = 0; //rolling count of unprocessed users

            //List to hold csv data
            List<Organization> organizationList; //= new List<Organization>();

            //Populate list with csv data
            organizationList = GetFileData<Organization, CustomOrganizationMap>(organizationFile, config);

            //save.SaveCHRISInformation(chrisData);

            //Return if nothing to store
            if (organizationList.Count == 0)
                return;

            //Iterate and save data
            foreach (Organization organizationData in organizationList)
            {
                //Save the data
                save.SaveOrganizationInformation(organizationData);

                //Increment counter
                processedRecords += 1;
            }

            //continue to process
        }

        /// <summary>
        /// Gets person id from db using hashed ssn
        /// The out param and the return are the same for some reason. 
        /// </summary>
        /// <param name="ssn"></param>
        /// <param name="personID"></param>
        /// <returns></returns>
        private int GetPersonID(byte[] ssn, out int personID)
        {
            //Open connection if not open
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                cmd.Connection = conn;
            }

            try
            {
                object obj;

                personID = 0;

                //Set query string
                cmd.CommandText = "Select pers_id from person where pers_hashed_ssn = @ssn";

                //Clear parameters and add sql parameter for ssn
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ssn", ssn);

                //Run command
                obj = cmd.ExecuteScalar();

                //Parse ID from object if not null
                if (obj != null)
                    personID = int.Parse(obj.ToString());

                //Return person id
                return personID;
            }
            //Catch all exceptions
            catch (Exception ex)
            {
                //Log error and re-throw
                log.Error(ex.Message + " - " + ex.InnerException);
                throw;
            }
        }

        //Overload in case we just want to return without an out
        private int GetPersonID(byte[] ssn)
        {
            try
            {
                using (conn)
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    using (cmd)
                    {
                        cmd.Connection = conn;

                        object obj;

                        cmd.CommandText = "Select pers_id from person where pers_hashed_ssn = @ssn";

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@ssn", ssn);

                        obj = cmd.ExecuteScalar();

                        if (obj != null)
                            return int.Parse(obj.ToString());

                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + " - " + ex.InnerException);
                return 0;
            }            
        }

        /// <summary>
        /// Determines if employee exists by employee id
        /// </summary>
        /// <param name="empID"></param>
        /// <returns></returns>
        private bool DoesEmployeeExist(string empID)
        {
            try
            {
                using (connHR)
                {
                    if (connHR.State == ConnectionState.Closed)
                        connHR.Open();

                    using (cmdHR)
                    {
                        cmdHR.Connection = connHR;
                        cmdHR.CommandType = CommandType.Text;

                        object obj;

                        //Create query string
                        cmdHR.CommandText = "Select emp_id from employee where emp_id = @empID";

                        //Clear and set parameters
                        cmdHR.Parameters.Clear();
                        cmdHR.Parameters.AddWithValue("@empID", empID);

                        //Execute query
                        obj = cmdHR.ExecuteScalar();

                        //If object not null return true
                        if (obj != null)
                            return true;

                        //Else return false
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + " - " + ex.InnerException);
                throw;
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

            //Used for testing
            //while (true)
            //{
                //var row = csvParser.Read();
                //if (row == null)
                //{
                    //Console.WriteLine(row[0] + " - " + row[1] + " - " + row[2]);
                    //break;
                //}
            //}

            csvReader.Configuration.RegisterClassMap<TMap>();

            return csvReader.GetRecords<TClass>().ToList();
        }
    }
}