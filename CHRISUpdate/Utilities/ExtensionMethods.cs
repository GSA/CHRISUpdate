using System.Text.RegularExpressions;

namespace HRUpdate.Utilities
{
    public static class ExtensionMethods
    {
        public static string RemovePhoneFormatting(this string s)
        {
            return Regex.Replace(s, "[^0-9]", string.Empty);
        }

        public static string RemoveSocialFormatting(this string s)
        {
            return Regex.Replace(s, "[^0-9]", string.Empty);
        }

        /// <summary>
        /// If first letter equals O return A, if W return P, otherwise return first letter
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string DetermineMajorOrg(this string s)
        {
            string officeSymbol = string.Empty;

            officeSymbol = Regex.Match(s, "[A-Za-z]").Value;

            switch (officeSymbol)
            {
                case "O":
                    return "A";

                case "W":
                    return "P";

                default:
                    return officeSymbol;
            }
        }

        //public static string RemoveZipFormatting
    }
}