using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NeoSpaceApp.Extensions.Helpers
{
    class ParseHelper
    {
        public static String GetBetween(string source, string s1, string s2)
        {
            if (string.IsNullOrWhiteSpace(source))
                return "";
            int idx1 = source.IndexOf(s1);
            int idx2 = source.IndexOf(s2, idx1 + s1.Length);
            if (idx1 == -1 || idx2 == -1)
                return "";
            return source.Substring(idx1 + s1.Length, idx2 - (idx1 + s1.Length));
        }

        public static List<String> GetBetweens(string source, string s1, string s2)
        {
            int index = 0;
            List<String> result = new List<string>();
            while (index < source.Length)
            {
                int idx1 = source.IndexOf(s1, index);
                if (idx1 == -1)
                    return result;
                int idx2 = source.IndexOf(s2, idx1 + s1.Length);
                if (idx1 != -1 && idx2 != -1)
                {
                    result.Add(source.Substring(idx1 + s1.Length, idx2 - (idx1 + s1.Length)));
                    index = idx2;
                }
                else
                    index = source.Length + 1;
            }
            return result;
        }

        public static string Clean(String source)
        {
            string result = source;
            result = Regex.Replace(result, @"(\s){2,}", " ", RegexOptions.IgnoreCase);
            result = result.Replace("<br />", "\n");
            result = result.Replace("<br/>", "\n");
            result = result.Replace("<br>", "\n");
            result = result.Replace("<p>", "\n");
            result = result.Replace("\t", "");
            while (Regex.IsMatch(result, @"<([\/a-z0-9!\-_&!#\s?:',\. ""\\=;]*)>", RegexOptions.IgnoreCase))
                result = Regex.Replace(result, @"<([\/a-z0-9!\-_&!#\s?:',\. ""\\=;]*)>", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"\n( )+", "\n", RegexOptions.IgnoreCase);
            return result.Trim();
        }
    }
}
