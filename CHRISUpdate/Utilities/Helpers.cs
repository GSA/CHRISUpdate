using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HRUpdate.Utilities
{
    internal class Helpers
    {
        /// <summary>
        /// Hashes SSN
        /// </summary>
        /// <param name="ssn"></param>
        /// <returns></returns>
        public byte[] HashSSN(string ssn)
        {
            byte[] hashedFullSSN = null;

            SHA256 shaM = new SHA256Managed();

            ssn = ssn.Replace("-", string.Empty);

            using (shaM)
            {
                if (ssn.Length == 9)
                    hashedFullSSN = shaM.ComputeHash(Encoding.UTF8.GetBytes(ssn));
            }

            return hashedFullSSN;
        }

        public void CopyValues<T>(T target, T source, params string [] excluded  )
        {
            if (target == null) return;
            Type t = typeof(T);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite).ToArray();

            for (int x = 0; x < properties.Count(); x++)
            {
                Type t2 = properties[x].GetValue(target, null).GetType();
                Type s2 = properties[x].GetValue(source, null).GetType();

                var childTargetProperties = t2.GetProperties().Where(p => p.CanRead && p.CanWrite).ToArray();
                var childSourceProperties = s2.GetProperties().Where(p => p.CanRead && p.CanWrite).ToArray();

                for (int y = 0; y < childTargetProperties.Count(); y++)
                {
                    if(!excluded.Any(a => a==childSourceProperties[y].Name))
                    {
                        var sourcevalue = childSourceProperties[y].GetValue(properties[x].GetValue(source, null), null);
                        var targetValue = childTargetProperties[y].GetValue(properties[x].GetValue(target, null), null);

                        if (falsy(targetValue, sourcevalue))
                        {
                            childTargetProperties[y].SetValue(properties[x].GetValue(target, null), sourcevalue);
                        }
                    }
                    
                }
            }
        }

        private bool falsy(object target, object source)
        {
            string t;
            if (target != null)
                t = target.GetType().ToString();
            else
                t = null;
            switch (t)
            {
                case "System.String":
                    {
                        string targetObj = target as string;
                        string sourceObj = source as string;
                        targetObj = targetObj == null ? "" : targetObj;
                        sourceObj = sourceObj == null ? "" : sourceObj;
                        if( targetObj.ToLower().Trim().Equals(sourceObj.ToLower().Trim()) )
                        {
                            return true;
                        }
                        return string.IsNullOrEmpty((string)target);
                    } 
                case "System.DateTime": return (DateTime)target == null || (DateTime)target == DateTime.MinValue;
                case "System.Boolean": return (bool?)target == null;
                case "System.Int64": return false;
                default: return true;
            }
        }
    }
}