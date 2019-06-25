using System.Text.RegularExpressions;

namespace HRUpdate.Utilities
{
    internal static class CsvFixer
    {
        public static string FixRecord(string record)
        {
            foreach (Match o in new Regex(@"[ ]"".+?""[ ]").Matches(record))
            {
                record = record.Replace(o.Value, o.Value.Replace('"', ' '));
            }
            return record;
        }
    }
}
