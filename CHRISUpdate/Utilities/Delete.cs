using Ionic.Zip;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRUpdate.Utilities
{
    public class Delete
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal void DeleteProcessFiles(string hrFilePath, string separationFilePath)
        {
            //Action is determined by DELETE setting in appsettings.config
            bool delete = ConfigurationManager.AppSettings["DELETE"].ToString().ToLower() == "on";

            if (delete)
            {
                log.Info("Deleting processed files: {0}");
                File.Delete(hrFilePath);
                File.Delete(separationFilePath);
            }
            else //zip, encrypt, and store file
            {
                encryptAndDelete(hrFilePath);
                encryptAndDelete(separationFilePath);
            }
        }

        private void encryptAndDelete(string file)
        {
            var folderPath = Path.GetDirectoryName(file);
            var today = DateTime.Now.ToString("yyyyMMdd");
            var uncompressedFolderName = Path.GetFileNameWithoutExtension(file) + "_" + today;
            var compressedFileName = uncompressedFolderName + ".zip";
            string pwd = ConfigurationManager.AppSettings["ZIPPASSWORD"].ToString();
            string folder = Path.Combine(folderPath, ConfigurationManager.AppSettings["ZIPFOLDERNAME"].ToString());
            string destination = Path.Combine(folder, compressedFileName);

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
                    zip.AddFile(file, uncompressedFolderName);
                    zip.Save(destination);
                }
                log.Info(string.Format("Successfully zipped file: {0}", file));
            }
            log.Info(string.Format("Deleting file: {0}", file));
            File.Delete(file);

        }
    }
}
