using System.Text.RegularExpressions;

namespace DataDesensitizer.FieldTypeProcessors;
public static class StringExtensions
{
    public static string StripNonAlphaNumericChars(this string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return value;

        return Regex.Replace(value, "[^a-zA-Z0-9]", String.Empty);
    }
}