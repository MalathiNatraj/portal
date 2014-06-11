using System.Text.RegularExpressions;

namespace Diebold.Services.Extensions
{
    public static class StringExtensions
    {
        public static string SeparatedCamelCase(this string theString)
        {
            return Regex.Replace(theString, "([A-Z]|[0-9]+)", " $1", RegexOptions.Compiled).Trim();
        }

        public static string UppercaseFirst(this string theString)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(theString))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(theString[0]) + theString.Substring(1);
        }
    }
}
