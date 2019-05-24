using CsvHelper;
using CsvHelper.Configuration;
using HRUpdate.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace HRUpdate.Utilities
{
    internal class FileReader
    {
        public List<TClass> GetFileData<TClass, TMap>(string filePath, out List<string> badRecords, ClassMap<Employee> employeeMap=null)
            where TClass : class
            where TMap : ClassMap<TClass>
        {
            //fix errors in file before processing
            using (var fs = new FileStream(filePath,FileMode.Open, FileAccess.ReadWrite))
            {
                var buffer = new byte[fs.Length];
                fs.ReadAsync(buffer, 0, Convert.ToInt32(fs.Length));
                var fileText = CsvFixer.FixRecord(new string(Encoding.UTF8.GetChars(buffer)));
                fs.WriteAsync(Encoding.UTF8.GetBytes(fileText), 0,fileText.Length);
            }

            using (var sr = new StreamReader(filePath))
            {
                using (var csvParser = new CsvParser(sr, true))
                {
                    var csvReader = new CsvReader(csvParser);
                    csvReader.Configuration.Delimiter = "~";
                    csvReader.Configuration.HasHeaderRecord = false;
                    if (employeeMap != null)
                    {
                        csvReader.Configuration.RegisterClassMap(employeeMap);
                    }
                    else
                    {
                        csvReader.Configuration.RegisterClassMap<TMap>();
                    }
                    var good = new List<TClass>();
                    var bad = new List<string>();
                    var isRecordBad = false;
                    csvReader.Configuration.BadDataFound = context =>
                    {
                        isRecordBad = true;
                        bad.Add(context.RawRecord);
                    };

                    while (csvReader.Read())
                    {
                        var record = csvReader.GetRecord<TClass>();
                        if (!isRecordBad)
                        {
                            good.Add(record);
                        }

                        isRecordBad = false;
                    }
                    badRecords = bad;

                    return good;
                }
            }
        }
    }

    internal class SummaryFileGenerator
    {
        //Reference to logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal string GenerateSummaryFile<TClass, TMap>(string fileName, IEnumerable<TClass> summaryData)
            where TClass : class
            where TMap : ClassMap<TClass>
        {
            try
            {
                var summaryFileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss_FFFF") + ".csv";

                //Creates the summary file
                using (CsvWriter csvWriter = new CsvWriter(new StreamWriter(ConfigurationManager.AppSettings["SUMMARYFILEPATH"] + summaryFileName, false)))
                {
                    csvWriter.Configuration.RegisterClassMap<TMap>();
                    csvWriter.WriteRecords(summaryData);
                }

                return summaryFileName;
            }
            catch (Exception ex)
            {
                Log.Error("Error Writing Summary File: " + fileName + " - " + ex.Message + " - " + ex.InnerException);
                return string.Empty;
            }
        }
    }
}