using HRUpdate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using HRUpdate.Process;
using HRUpdate.Validation;
using KellermanSoftware.CompareNetObjects;
using log4net;

namespace HRUpdate.Utilities
{
    internal static class Helpers
    {
        /// <summary>
        /// Hashes SSN
        /// </summary>
        /// <param name="ssn"></param>
        /// <returns></returns>
        public static byte[] HashSsn(string ssn)
        {
            byte[] hashedFullSsn = null;

            SHA256 shaM = new SHA256Managed();

            ssn = ssn.Replace("-", string.Empty);

            using (shaM)
            {
                    hashedFullSsn = shaM.ComputeHash(Encoding.UTF8.GetBytes(ssn));
            }

            return hashedFullSsn;
        }

        /// <summary>
        /// Function to copy the values of 2 objects of the same type using a predefined set of rules
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="excluded"></param>
        public static void CopyValues<T>(T target, T source, params string[] excluded)
        {
            if (target == null) return;
            var t = typeof(T);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite).ToArray();

            for (var x = 0; x < properties.Count(); x++)
            {
                var t2 = properties[x].GetValue(target, null).GetType();
                var s2 = properties[x].GetValue(source, null).GetType();

                var childTargetProperties = t2.GetProperties().Where(p => p.CanRead && p.CanWrite).ToArray();
                var childSourceProperties = s2.GetProperties().Where(p => p.CanRead && p.CanWrite).ToArray();

                for (var y = 0; y < childTargetProperties.Count(); y++)
                {
                    if (excluded.Any(a => a == childSourceProperties[y].Name)) continue;
                    var sourceValue = childSourceProperties[y].GetValue(properties[x].GetValue(source, null), null);
                    var targetValue = childTargetProperties[y].GetValue(properties[x].GetValue(target, null), null);

                    if (Falsy(targetValue, sourceValue))
                    {
                        childTargetProperties[y].SetValue(properties[x].GetValue(target, null), sourceValue);
                    }
                }
            }
        }

        /// <summary>
        /// Uses variable type to help determine the copy action of a column
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private static bool Falsy(object target, object source)
        {
            var t = target?.GetType().ToString();
            switch (t)
            {
                case "System.String":
                    {
                        var targetObj = target as string;
                        var sourceObj = source as string;
                        targetObj = targetObj ?? "";
                        sourceObj = sourceObj ?? "";
                        return targetObj.ToLower().Trim().Equals(sourceObj.ToLower().Trim()) || string.IsNullOrEmpty((string)target);
                    }
                case "System.DateTime": return (DateTime)target == null || (DateTime)target == DateTime.MinValue;
                case "System.Boolean": return (bool?)target == null;
                case "System.Int64": return false;
                default: return true;
            }
        }

        /// <summary>
        /// Removes pound signs from the 3 personal addresses
        /// </summary>
        /// <param name="a"></param>
        private static void CleanAddress(Address a)
        {
            var s = new[] { "#" };
            a.HomeAddress1 = a.HomeAddress1.removeItems(s);
            a.HomeAddress2 = a.HomeAddress2.removeItems(s);
            a.HomeAddress3 = a.HomeAddress3.removeItems(s);
            a.HomeCity = a.HomeCity.removeItems(s);            
        }

        /// <summary>
        /// Formats phone numbers to be in correct format to insert into db
        /// </summary>
        /// <param name="employeeData"></param>
        public static void CleanupHrData(Employee employeeData)
        {
            //Address clean up
            CleanAddress(employeeData.Address);

            //Phone clean up
            employeeData.Phone.WorkFax = employeeData.Phone.WorkFax.RemovePhoneFormatting();
            employeeData.Phone.WorkCell = employeeData.Phone.WorkCell.RemovePhoneFormatting();
            employeeData.Phone.WorkPhone = employeeData.Phone.WorkPhone.RemovePhoneFormatting();
            employeeData.Phone.WorkTextTelephone = employeeData.Phone.WorkTextTelephone.RemovePhoneFormatting();
            employeeData.Phone.HomeCell = employeeData.Phone.HomeCell.RemovePhoneFormatting();
            employeeData.Phone.HomePhone = employeeData.Phone.HomePhone.RemovePhoneFormatting();

            //Emergency contact clean up
            employeeData.Emergency.EmergencyContactHomePhone = employeeData.Emergency.EmergencyContactHomePhone.RemovePhoneFormatting();
            employeeData.Emergency.EmergencyContactWorkPhone = employeeData.Emergency.EmergencyContactWorkPhone.RemovePhoneFormatting();
            employeeData.Emergency.EmergencyContactCellPhone = employeeData.Emergency.EmergencyContactCellPhone.RemovePhoneFormatting();
            employeeData.Emergency.OutOfAreaContactHomePhone = employeeData.Emergency.OutOfAreaContactHomePhone.RemovePhoneFormatting();
            employeeData.Emergency.OutOfAreaContactWorkPhone = employeeData.Emergency.OutOfAreaContactWorkPhone.RemovePhoneFormatting();
            employeeData.Emergency.OutOfAreaContactCellPhone = employeeData.Emergency.OutOfAreaContactCellPhone.RemovePhoneFormatting();
        }

        /// <summary>
        /// Adds a bad record to the summary
        /// </summary>
        /// <param name="badRecords"></param>
        /// <param name="summary"></param>
        public static void AddBadRecordsToSummary(IEnumerable<string> badRecords, ref HRSummary summary)
        {
            foreach (var item in badRecords)
            {
                var parts = new List<string>();
                var s = item.removeItems(new[] { "\"" });
                parts.AddRange(s.Split('~'));
                var obj = new ProcessedSummary
                {
                    GCIMSID = -1,
                    Action = "Invalid Record From CSV File",
                    EmployeeID = parts.Count > 0 ? parts[0] : "Unknown Employee Id",
                    LastName = parts.Count > 1 ? parts[1] : "Unknown Last Name",
                    Suffix = parts.Count > 2 ? parts[2] : "Unknown Suffix",
                    FirstName = parts.Count > 3 ? parts[3] : "Unknown First Name",
                    MiddleName = parts.Count > 4 ? parts[4] : "Unknown Middle Name"
                };
                summary.UnsuccessfulUsersProcessed.Add(obj);
            }
        }

        /// <summary>
        /// Returns an Employee object if match found in db
        /// </summary>
        /// <param name="employeeData"></param>
        /// <param name="allGcimsData"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static Employee RecordFound(Employee employeeData, List<Employee> allGcimsData, ref ILog log)
        {
            var hrLinksMatch = allGcimsData.Where(w => employeeData.Person.EmployeeID == w.Person.EmployeeID).ToList();

            if (hrLinksMatch.Count > 1)
            {
                log.Info("Multiple HR Links IDs Found: " + employeeData.Person.EmployeeID);

                return null;
            }
            else if (hrLinksMatch.Count == 1)
            {
                log.Info("Matching record found by emplID: " + employeeData.Person.EmployeeID);

                return hrLinksMatch.Single();
            }
            else if (hrLinksMatch.Count == 0)
            {
                log.Info("Trying to match record by Lastname, Birth Date and SSN: " + employeeData.Person.EmployeeID);

                var nameMatch = allGcimsData.Where(w =>
                    employeeData.Person.LastName.ToLower().Trim().Equals(w.Person.LastName.ToLower().Trim()) &&
                    employeeData.Person.SocialSecurityNumber.Equals(w.Person.SocialSecurityNumber) &&
                    employeeData.Birth.DateOfBirth.Equals(w.Birth.DateOfBirth)).ToList();

                if (nameMatch.Count == 0 || nameMatch.Count > 1)
                {
                    log.Info("Match not found by name for user: " + employeeData.Person.EmployeeID);
                    return null;
                }
                else if (nameMatch.Count == 1)
                {
                    log.Info("Match found by name for user: " + employeeData.Person.EmployeeID);
                    return nameMatch.Single();
                }
            }

            return null;
        }

        /// <summary>
        /// Determines if 2 same type object are equal. Fields can be ignored
        /// </summary>
        /// <param name="gcimsData"></param>
        /// <param name="hrData"></param>
        /// <param name="propertyNameList"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static bool AreEqualGcimsToHr(Employee gcimsData, Employee hrData, out string propertyNameList, ref ILog log)
        {
            var compareLogic = new CompareLogic
            {
                Config = {TreatStringEmptyAndNullTheSame = true, CaseSensitive = false, MaxDifferences = 100}
            };

            compareLogic.Config.CustomComparers.Add(new EmployeeComparer(RootComparerFactory.GetRootComparer()));

            var result = compareLogic.Compare(gcimsData, hrData);

            var diffs = result.Differences.Select(a => a.PropertyName).ToArray();
            var localPropertyNameList = string.Join(",", diffs);
            propertyNameList = localPropertyNameList;
            if (diffs?.Length > 0)
            {
                log.Info($"Property differences include: {localPropertyNameList}");
            }

            return result.AreEqual;
        }

        /// <summary>
        /// Processes the validation and returns if a record has validation errors
        /// </summary>
        /// <param name="validate"></param>
        /// <param name="employeeData"></param>
        /// <param name="unsuccessfulHrUsersProcessed"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static bool CheckForErrors(ValidateHR validate, Employee employeeData, List<ProcessedSummary> unsuccessfulHrUsersProcessed, ref ILog log)
        {
            var validationHelper = new ValidationHelper();
            var criticalErrors = validate.ValidateEmployeeCriticalInfo(employeeData);

            if (criticalErrors.IsValid) return false;
            log.Warn("Errors found for user: " + employeeData.Person.EmployeeID + "(" + criticalErrors.Errors.Count() + ")");

            unsuccessfulHrUsersProcessed.Add(new ProcessedSummary
            {
                GCIMSID = -1,
                EmployeeID = employeeData.Person.EmployeeID,
                FirstName = employeeData.Person.FirstName,
                MiddleName = employeeData.Person.MiddleName,
                LastName = employeeData.Person.LastName,
                Suffix = employeeData.Person.Suffix,
                Action = validationHelper.GetErrors(criticalErrors.Errors, ValidationHelper.Hrlinks.Hrfile).TrimEnd(',')
            });

            return true;

        }

        /// <summary>
        /// Copies the investigation data 
        /// </summary>
        /// <param name="hr"></param>
        /// <param name="db"></param>
        public static void InvestigationCopy(Employee hr, Employee db)
        {
            var hrValue = hr.Investigation.InitialResult;
            var hrDate = hr.Investigation.InitialResultDate;
            var dbValue = db.Investigation.InitialResult;
            var dbDate = db.Investigation.InitialResultDate;

            if (hrValue == false || (hrDate == null || hrDate == DateTime.MinValue))
            {
                hr.Investigation.InitialResult = dbValue;
                hr.Investigation.InitialResultDate = dbDate;
            }

            hrValue = hr.Investigation.FinalResult;
            hrDate = hr.Investigation.FinalResultDate;
            dbValue = db.Investigation.FinalResult;
            dbDate = db.Investigation.FinalResultDate;

            if (hrValue == false || (hrDate == null || hrDate == DateTime.MinValue))
            {
                hr.Investigation.FinalResult = dbValue;
                hr.Investigation.FinalResultDate = dbDate;
            }
        }
    }
}