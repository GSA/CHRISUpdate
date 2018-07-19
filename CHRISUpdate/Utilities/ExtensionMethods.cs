﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HRUpdate.Utilities
{
    public static class ExtensionMethods
    {
        public static void GetValFromKey<TKey,TVal>(this Dictionary<TKey,TVal> d, TKey s, ref object o)
        {
            if (d.ContainsKey(s))
                o =  d[s];             
        }

        public static string RemovePhoneFormatting(this string s)
        {
            return Regex.Replace(s, "[^0-9]", string.Empty);
        }

        public static string RemoveSocialFormatting(this string s)
        {
            return Regex.Replace(s, "[^0-9]", string.Empty);
        }

        public static string removeItems(this string old, string[] toRemove)
        {
            string s = old;
            foreach (var c in toRemove)
            {
                s = s.Replace(c, string.Empty);
            }

            return s;
        }
    }
}