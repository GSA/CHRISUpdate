using HRUpdate.Models;
using HRUpdate.Utilities;
using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace HRUpdate.Process
{
    internal class SendSummary
    {
        //Reference to logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly EMailData emailData = new EMailData();

        public SendSummary(ref EMailData emailData)
        {
            this.emailData = emailData;
        }

        public void SendSummaryEMail()
        {
            EMail email = new EMail();

            string subject = string.Empty;
            string body = string.Empty;
            string attahcments = string.Empty;

            subject = ConfigurationManager.AppSettings["EMAILSUBJECT"].ToString() + " - " + DateTime.Now.ToString("MMMM dd, yyyy HH:mm:ss");

            body = GenerateEMailBody();

            attahcments = SummaryAttachments();

            try
            {
                using (email)
                {
                    email.Send(ConfigurationManager.AppSettings["DEFAULTEMAIL"].ToString(),
                               ConfigurationManager.AppSettings["TO"].ToString(),
                               ConfigurationManager.AppSettings["CC"].ToString(),
                               ConfigurationManager.AppSettings["BCC"].ToString(),
                               subject, body, attahcments.TrimEnd(';'), ConfigurationManager.AppSettings["SMTPSERVER"].ToString(), true);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error Sending HR Links Summary E-Mail: " + ex.Message + " - " + ex.InnerException);
            }
            finally
            {
                log.Info("HR Links Summary E-Mail Sent");
            }
        }

        public string GenerateEMailBody()
        {
            StringBuilder errors = new StringBuilder();
            StringBuilder fileNames = new StringBuilder();

            string template = File.ReadAllText(ConfigurationManager.AppSettings["SUMMARYTEMPLATE"]);

            fileNames.Append(emailData.HRFilename == null ? "No HR Links File Found" : emailData.HRFilename.ToString());
            fileNames.Append(", ");
            fileNames.Append(emailData.SEPFileName == null ? "No Separation File Found" : emailData.SEPFileName.ToString());

            template = template.Replace("[FILENAMES]", fileNames.ToString());

            template = template.Replace("[HRATTEMPTED]", emailData.HRAttempted.ToString());
            template = template.Replace("[HRSUCCEEDED]", emailData.HRSucceeded.ToString());
            template = template.Replace("[HRIDENTICAL]", emailData.HRIdentical.ToString());
            template = template.Replace("[HRINACTIVE]", emailData.HRInactive.ToString());
            template = template.Replace("[HRRECORDSNOTFOUND]", emailData.HRRecordsNotFound.ToString());
            template = template.Replace("[HRFAILED]", emailData.HRFailed.ToString());

            if (emailData.HRHasErrors)
            {
                errors.Clear();

                errors.Append("<b><font color='red'>Errors were found while processing the HR file</font></b><br />");
                errors.Append("<br />Please see the attached file: <b><font color='red'>");
                errors.Append(emailData.HRUnsuccessfulFilename);
                errors.Append("</font></b>");

                template = template.Replace("[IFHRERRORS]", errors.ToString());
            }
            else
            {
                template = template.Replace("[IFHRERRORS]", null);
            }

            template = template.Replace("[SEPATTEMPTED]", emailData.SEPAttempted.ToString());
            template = template.Replace("[SEPSUCCEEDED]", emailData.SEPSucceeded.ToString());
            template = template.Replace("[SEPFAILED]", emailData.SEPFailed.ToString());

            if (emailData.SEPHasErrors)
            {
                errors.Clear();

                errors.Append("<b><font color='red'>Errors were found while processing the separation file</font></b><br />");
                errors.Append("<br />Please see the attached file: <b><font color='red'>");
                errors.Append(emailData.SeparationErrorFilename);
                errors.Append("</font></b>");

                template = template.Replace("[IFSEPERRORS]", errors.ToString());
            }
            else
            {
                template = template.Replace("[IFSEPERRORS]", null);
            }

            return template;
        }

        private string SummaryAttachments()
        {
            StringBuilder attachments = new StringBuilder();

            //HR Summary Files
            if (emailData.HRSuccessfulFilename != null)
                attachments.Append(AddAttachment(emailData.HRSuccessfulFilename));

            if (emailData.HRUnsuccessfulFilename != null)
                attachments.Append(AddAttachment(emailData.HRUnsuccessfulFilename));

            if (emailData.HRInactiveFilename != null)
                attachments.Append(AddAttachment(emailData.HRInactiveFilename));

            if (emailData.HRRecordsNotFoundFileName != null)
                attachments.Append(AddAttachment(emailData.HRRecordsNotFoundFileName));

            //Separation Summary Files
            if (emailData.SeparationSuccessfulFilename != null)
                attachments.Append(AddAttachment(emailData.SeparationSuccessfulFilename));

            if (emailData.SeparationErrorFilename != null)
                attachments.Append(AddAttachment(emailData.SeparationErrorFilename));

            return attachments.ToString();
        }

        private string AddAttachment(string fileName)
        {
            StringBuilder addAttachment = new StringBuilder();

            addAttachment.Append(ConfigurationManager.AppSettings["SUMMARYFILEPATH"]);
            addAttachment.Append(fileName);
            addAttachment.Append(";");

            return addAttachment.ToString();
        }
    }
}