using System.Security.Cryptography;
using System.Text;

namespace HRUpdate.Utilities
{
    class Helpers
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
    }
}
