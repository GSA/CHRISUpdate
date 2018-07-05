using HRUpdate.Models;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.CompareNetObjects.TypeComparers;
using System;
using System.Linq;

namespace HRUpdate.Utilities
{
    public class EmployeeComparer : BaseTypeComparer
    {
        public EmployeeComparer(RootComparer rootComparer) : base(rootComparer)
        {
        }

        public override bool IsTypeMatch(Type type1, Type type2)
        {
            return type1 == type2;
        }

        public override void CompareType(CompareParms parms)
        {
            string[] included = { "FinalResult", "InitialResult", "FinalResultDate", "InitialResultDate" };
            string[] excluded = { "GCIMSID", "FirstName", "MiddleName", "LastName", "Suffix", "Status", "SocialSecurityNumber" };
            var db = (Employee)parms.Object1;
            var hr = (Employee)parms.Object2;
            var properties = typeof(Employee).GetProperties().Where(prop => prop.CanRead && prop.CanWrite).ToArray();

            for (int x = 0; x < properties.Count(); x++)
            {
                Type t2 = properties[x].GetValue(db, null).GetType();
                Type s2 = properties[x].GetValue(hr, null).GetType();

                var childTargetProperties = t2.GetProperties().Where(p => p.CanRead && p.CanWrite).ToArray();
                var childSourceProperties = s2.GetProperties().Where(p => p.CanRead && p.CanWrite).ToArray();

                for (int y = 0; y < childTargetProperties.Count(); y++)
                {
                    var dbValue = childSourceProperties[y].GetValue(properties[x].GetValue(db, null), null);
                    var hrValue = childTargetProperties[y].GetValue(properties[x].GetValue(hr, null), null);

                    if (included.Any(q => q == childSourceProperties[y].Name))
                    {
                        //code for initial/final investigations
                    }
                    else
                    {
                        if (!excluded.Any(z => z == childSourceProperties[y].Name))
                        {
                            string t;
                            if (hrValue != null)
                                t = hrValue.GetType().ToString();
                            else
                                t = null;
                            switch (t)
                            {
                                case "System.String":
                                    {
                                        string targetObj = dbValue as string;
                                        string sourceObj = hrValue as string;
                                        targetObj = targetObj == null ? "" : targetObj;
                                        sourceObj = sourceObj == null ? "" : sourceObj;
                                        if ((!targetObj.ToLower().Trim().Equals(sourceObj.ToLower().Trim())) && !string.IsNullOrWhiteSpace(sourceObj))
                                        {
                                            Difference difference = new Difference
                                            {
                                                PropertyName = childSourceProperties[y].Name,
                                                Object1Value = dbValue == null ? "" : dbValue.ToString(),
                                                Object2Value = hrValue == null ? "" : hrValue.ToString()
                                            };
                                            parms.Result.Differences.Add(difference);
                                        }
                                    }
                                    break;

                                case "System.DateTime":
                                    {
                                        var targetObj = dbValue as DateTime?;
                                        var sourceObj = hrValue as DateTime?;
                                        if (targetObj != sourceObj && (sourceObj == null || sourceObj != DateTime.MinValue))
                                        {
                                            Difference difference = new Difference
                                            {
                                                PropertyName = childSourceProperties[y].Name,
                                                Object1Value = dbValue == null ? "" : dbValue.ToString(),
                                                Object2Value = hrValue == null ? "" : hrValue.ToString()
                                            };
                                            parms.Result.Differences.Add(difference);
                                        }
                                    }
                                    break;

                                case "System.Boolean":
                                    {
                                        var targetObj = dbValue as bool?;
                                        var sourceObj = hrValue as bool?;
                                        if (targetObj != sourceObj)
                                        {
                                            Difference difference = new Difference
                                            {
                                                PropertyName = childSourceProperties[y].Name,
                                                Object1Value = dbValue == null ? "" : dbValue.ToString(),
                                                Object2Value = hrValue == null ? "" : hrValue.ToString()
                                            };
                                            parms.Result.Differences.Add(difference);
                                        }
                                    }
                                    break;

                                case "System.Int64":
                                    {
                                        var targetObj = dbValue as Int64?;
                                        var sourceObj = hrValue as Int64?;
                                        if (targetObj != sourceObj && (sourceObj != 0))
                                        {
                                            Difference difference = new Difference
                                            {
                                                PropertyName = childSourceProperties[y].Name,
                                                Object1Value = dbValue == null ? "" : dbValue.ToString(),
                                                Object2Value = hrValue == null ? "" : hrValue.ToString()
                                            };
                                            parms.Result.Differences.Add(difference);
                                        }
                                    }
                                    break;

                                case null:
                                    //nothing
                                    break;

                                default:
                                    {
                                        Difference difference = new Difference
                                        {
                                            PropertyName = childSourceProperties[y].Name,
                                            Object1Value = dbValue == null ? "" : dbValue.ToString(),
                                            Object2Value = hrValue == null ? "" : hrValue.ToString()
                                        };
                                        parms.Result.Differences.Add(difference);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}