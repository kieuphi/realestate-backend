using System.Text.RegularExpressions;

namespace General.Domain.Common.Excel
{
    public static class StringExtensions
    {
        public static string TrimAndUppercase(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }

            return s.Trim().ToUpper();
        }

        public static string TrimAndLowercase(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }

            return s.Trim().ToLower();
        }

        public static string SplitCamelCase(this string str)
        {
            return Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }
    }
}
