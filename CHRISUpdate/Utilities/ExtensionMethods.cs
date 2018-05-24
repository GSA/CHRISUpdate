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
    }
}