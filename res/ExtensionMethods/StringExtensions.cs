using System;

namespace PBRP
{
    public static class StringExtensions
    {
        public static string Roleplay(this string userName)
        {
            return System.Text.RegularExpressions.Regex.Replace(userName, "_", " ");
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source != null && toCheck != null && source.IndexOf(toCheck, comp) >= 0;
        }
    }
}
