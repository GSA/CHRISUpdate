using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace HRUpdate.Utilities
{
    public class Delete
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal void DeleteProcessFiles(string hrFilePath, string separationFilePath, Dictionary<string, object> objects)
        {

            encryptAndDelete(hrFilePath);

            //DeleteFile(separationFilePath);

            MoveFile(separationFilePath);

        }

        public void MoveFile(string file)
        {
            var folderPath = Path.GetDirectoryName(file);
            var FileName = Path.GetFileNameWithoutExtension(file) + "_moved.csv";
            string folder = Path.Combine(folderPath, ConfigurationManager.AppSettings["ZIPFOLDERNAME"].ToString());
            string destination = Path.Combine(folder, FileName);

            Directory.CreateDirectory(folder);

            if(File.Exists(file))
            {
                try
                {
                    log.Info(string.Format("Attempting to move file: {0}", file));
                    File.Move(file, destination);
                    log.Info(string.Format("File move to path {0}", destination));
                }
                catch (IOException e)
                {
                    log.Warn(string.Format("Moving file {0} failed.", file));
                }
                
            }
            
        }

        private void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    log.Info(string.Format("Deleting file: {0}", filePath));
                    File.Delete(filePath);
                }
                catch (Exception e)
                {
                    log.Warn(string.Format("Unable to delete file: {0}", filePath), e);
                }
            }
        }

        private void encryptAndDelete(string file)
        {
            if (File.Exists(file))
            {
                var folderPath = Path.GetDirectoryName(file);
                var FileName = Path.GetFileNameWithoutExtension(file) + "_encrypted.zip";
                string pwd = ConfigurationManager.AppSettings["ZIPPASSWORD"].ToString();
                string folder = Path.Combine(folderPath, ConfigurationManager.AppSettings["ZIPFOLDERNAME"].ToString());
                string destination = Path.Combine(folder, FileName);

                Directory.CreateDirectory(folder);

                if (string.IsNullOrWhiteSpace(pwd))
                {
                    log.Info(string.Format("No password submitted, attempting to zip file: {0}", file));
                    using (ZipFile zip = new ZipFile())
                    {
                        zip.AddFile(file);
                        zip.Save(destination);
                    }
                    log.Info(string.Format("Successfully zipped file with no password: {0}", file));
                }
                else
                {
                    log.Info(string.Format("Attempting to zip and encrypt file: {0}", file));
                    using (ZipFile zip = new ZipFile())
                    {
                        zip.Password = pwd;
                        zip.Encryption = EncryptionAlgorithm.WinZipAes256;
                        zip.AddFile(file, FileName);
                        zip.Save(destination);
                    }
                    log.Info(string.Format("Successfully zipped file: {0}", file));
                }
                log.Info(string.Format("File saved to: {0}", destination));
                DeleteFile(file);
            }
            else
            {
                log.Info("No file found to encrypt and delete.");
            }
        }
    }
}
