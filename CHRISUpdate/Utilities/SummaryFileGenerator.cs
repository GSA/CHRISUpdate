using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace HRUpdate.Utilities
{
    class SummaryFileGenerator
    {
        //Reference to logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Empty Contructor
        public SummaryFileGenerator() { }

        public void GenerateSummaryFile<TClass,TMap>(string fileName, List<TClass> summaryData)
            where TClass : class
            where TMap : ClassMap<TClass>
        {
            try
            {   
                //Creates the summary file
                using (CsvWriter csvWriter = new CsvWriter(new StreamWriter(ConfigurationManager.AppSettings["SUMMARYFILEPATH"] + fileName, false)))
                {
                    csvWriter.Configuration.RegisterClassMap<TMap>();
                    csvWriter.WriteRecords(summaryData);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error Writing Summary File: " + fileName + " - " + ex.Message + " - " + ex.InnerException);
            }
        }        
    }
}
