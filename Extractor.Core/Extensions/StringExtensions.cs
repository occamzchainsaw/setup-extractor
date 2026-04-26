using System.Text.RegularExpressions;

namespace Extractor.Core.Extensions;

public static class StringExtensions
{
    extension(string str)
    {
        public string SanitizeSpecialChars()
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            string s = str.ToLowerInvariant();
            s = Regex.Replace(s, "[^a-z0-9]", "");

            return s;
        }
    }
}