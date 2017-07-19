using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using CHRISUpdate.Mapping;
using CHRISUpdate.Models;
using CsvHelper;
using CsvHelper.Configuration;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace CHRISUpdate.Process
{
    //private static Stopwatch timeForProcesses = new Stopwatch();
    class ProcessData
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static CsvConfiguration config = new CsvConfiguration();

        private MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GCIMS"].ToString());
        private MySqlCommand cmd = new MySqlCommand();

        private MySqlConnection connHR = new MySqlConnection(ConfigurationManager.ConnectionStrings["HR"].ToString());
        private MySqlCommand cmdHR = new MySqlCommand();

        private static List<Chris> chrisList = new List<Chris>();
        private static List<ChrisData> chrisListData = new List<ChrisData>();

        int processedRecords = 0; //rolling count of records that were processed
        int processedUsers = 0;  //rolling count of users processed
        int unprocessedUsers = 0; //rolling count of unprocessed users

        SaveData save = new SaveData();

        //Need better naming namespace and convention here
        Utilities.Utilities u = new Utilities.Utilities();

        //Set up stuff when it's intalized
        public ProcessData()
        {
            config.Delimiter = "~";
            config.HasHeaderRecord = false;            
            config.WillThrowOnMissingField = false;
        }

        public void ProcessCHRISFile(string chrisFile)
        {
            log.Info("Processing CHRIS Users");

            try
            {                              
                chrisList = GetFileData<Chris, CHRISMapping>(chrisFile, config);                

                foreach (Chris chrisData in chrisList) // Loop through List with foreach - Chris chrisData in chrisList
                {
                    int personID = 0;

                    chrisData.Employee.SSN = u.HashSSN(chrisData.SSN);

                    if (GetPersonID(chrisData.Employee.SSN, out personID) > 0)
                    {
                        chrisData.Employee.PersonID = personID;

                        //PersonID and Set Supervisor name (we are not able to map this as it's a combined field)
                        //Reason personID is apart of person is we can just pass the object to the save method
                        chrisData.Person.PersonID = personID;
                        chrisData.Person.Supervisor = chrisData.Supervisor.LastNameSuffix + ", " + chrisData.Supervisor.FirstName + " " + chrisData.Supervisor.MiddleName;

                        save.SaveCHRISInformation(chrisData, connHR);

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
                        log.Warn("Not Found! " + chrisData.Employee.EmployeeID + " " + chrisData.Employee.FirstName + " " + chrisData.Employee.FamilySuffix);
                    }

                    processedRecords += 1;
                }

                log.Info("CHRIS Records Processed: " + String.Format("{0:#,###0}", processedUsers));
                log.Info("CHRIS Users Not Processed: " + String.Format("{0:#,###0}", unprocessedUsers));
                log.Info("CHRIS Processed Records: " + String.Format("{0:#,###0}", processedRecords));

                connHR.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                log.Error("Process CHRIS Users Error:" + ex.Message + " " + ex.InnerException + " " + ex.StackTrace);                
            }
        }

        public void ProcessSeparationFile(string separationFile)
        {
            log.Info("Processing Separation Users");

            //If we can't open the DB just crash here no point going forward (just making sure we have a connection)
            if (connHR.State == ConnectionState.Closed)
            {
                connHR.Open();
                cmdHR.Connection = connHR;
            }

            processedRecords = 0; //rolling count of records that were processed
            processedUsers = 0;  //rolling count of users processed
            unprocessedUsers = 0; //rolling count of unprocessed users
            
            List<Separation> separationList = new List<Separation>();

            separationList = GetFileData<Separation, CustomSeparationMap>(separationFile, config);

            try
            {
                foreach (Separation separationData in separationList)
                {
                    if (DoesEmployeeExist(separationData.EmployeeID)) //EmployeeID = Chris ID
                    {
                        save.SaveSeparationInformation(separationData, connHR);                       

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
                        log.Warn("Not Found! " + separationData.EmployeeUniqueID + " " + separationData.FirstName + " " + separationData.LastNameAndSuffix);
                    }

                    //Moved here on 9/15 was in the wrong location and was reporting 1 always
                    processedRecords += 1;  
                }            

                log.Info("Separation Records Processed: " + String.Format("{0:#,###0}", processedUsers));
                log.Info("Separation Users Not Processed: " + String.Format("{0:#,###0}", unprocessedUsers));
                log.Info("Separation Processed Records: " + String.Format("{0:#,###0}", processedRecords));
            }
            catch (Exception ex)
            {
                log.Error("Process Separation Users Error:" + ex.Message + " " + ex.InnerException); 
            }
        }

        public void ProcessOrganizationFile(string organizationFile)
        {
            log.Info("Processing Separation Users");

            //If we can't open the DB just crash here no point going forward (just making sure we have a connection)
            //if (conn.State == ConnectionState.Closed)
            //{
            //    conn.Open();
            //    cmd.Connection = conn;
            //}

            processedRecords = 0; //rolling count of records that were processed
            processedUsers = 0;  //rolling count of users processed
            unprocessedUsers = 0; //rolling count of unprocessed users

            List<Organization> organizationList = new List<Organization>();

            organizationList = GetFileData<Organization, CustomOrganizationMap>(organizationFile, config);

            //save.SaveCHRISInformation(chrisData);           

            if (organizationList.Count == 0)
                return;

            foreach (Organization organizationData in organizationList)
            {
                save.SaveOrganizationInformation(organizationData);

                processedRecords += 1;
            }

            //continue to process
        }

        private int GetPersonID(byte[] ssn, out int personID)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                cmd.Connection = conn;
            }

            try
            {   
                object obj;

                personID = 0;

                cmd.CommandText = "Select pers_id from person where pers_hashed_ssn = @ssn";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ssn", ssn);

                obj = cmd.ExecuteScalar();

                if (obj != null)
                    personID = int.Parse(obj.ToString());
                    
                return personID;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + " - " + ex.InnerException);
                throw;
            }
        }

        //Overload incase we just want to return without an out
        private int GetPersonID(byte[] ssn)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                cmd.Connection = conn;
            }

            try
            {
                object obj;

                cmd.CommandText = "Select pers_id from person where pers_hashed_ssn = @ssn";

                cmd.Parameters.Clear();                
                cmd.Parameters.AddWithValue("@ssn", ssn);

                obj = cmd.ExecuteScalar();

                if (obj != null)
                    return int.Parse(obj.ToString());

                return 0;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + " - " + ex.InnerException);
                throw;
            }
        }

        private bool DoesEmployeeExist(string empID)
        {
            if (connHR.State == ConnectionState.Closed)
            {
                connHR.Open();
                cmdHR.Connection = connHR;
            }

            try
            {
                object obj;

                cmdHR.CommandText = "Select emp_id from employee where emp_id = @empID";

                cmdHR.Parameters.Clear();
                cmdHR.Parameters.AddWithValue("@empID", empID);

                obj = cmdHR.ExecuteScalar();

                if (obj != null)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + " - " + ex.InnerException);
                throw;
            }
        }


        private List<TClass> GetFileData<TClass, TMap>(string filePath, CsvConfiguration config)
            where TClass : class
            where TMap : CsvClassMap<TClass>
        {
            CsvParser csvParser = new CsvParser(new StreamReader(filePath), config);
            CsvReader csvReader = new CsvReader(csvParser);

            //Used for testing
            //while (true)
            //{
            //    var row = csvParser.Read();
            //    if (row == null)
            //    {
            //        break;
            //    }
            //}
            
            csvReader.Configuration.RegisterClassMap<TMap>();

            return csvReader.GetRecords<TClass>().ToList();
        }
    }
}