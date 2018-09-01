using System.Text.RegularExpressions;

namespace Bidster
{
    public static class ExtensionMethods
    {
        public static string Clean(this string input)
        {
            var regex = new Regex("[^a-z0-9\\-_]", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

            input = input.Replace(" ", "-");
            var cleaned = regex.Replace(input, "").ToLower();

            while (cleaned.Contains("--"))
            {
                cleaned = cleaned.Replace("--", "-");
            }

            return cleaned;
        }
    }
}