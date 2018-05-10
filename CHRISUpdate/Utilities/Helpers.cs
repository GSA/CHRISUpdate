using System;
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

        public void CopyValues<T>(T target, T source)
        {
            Type t = typeof(T);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var sourcevalue = prop.GetValue(source, null);
                var targetValue = prop.GetValue(target, null);

                if (falsy(targetValue))
                {
                    prop.SetValue(target, sourcevalue, null);
                }
            }
        }

        private bool falsy(object val)
        {
            string t;
            if (val != null)
                t = val.GetType().ToString();
            else
                t = null;
            Console.WriteLine(t);
            switch (t)
            {
                case "System.String": return string.IsNullOrEmpty((string)val);
                case "System.DateTime": return (DateTime)val == null || (DateTime)val == DateTime.MinValue;
                case "System.Boolean": return (bool?)val == null;
                default: return true;
            }
        }
    }
}