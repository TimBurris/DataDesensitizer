using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Text;

namespace DataDesensitizer.FieldTypeProcessors;

public class SocialSecurityNumberFieldTypeProcessor : Abstractions.IFieldTypeProcessor
{
    private readonly ILogger<PhoneNumberFieldTypeProcessor> _logger;

    public SocialSecurityNumberFieldTypeProcessor(FieldTypeProcessorSettings settings, ILogger<PhoneNumberFieldTypeProcessor> logger)
    {
        _logger = logger;
    }

    public string Name => "SSN";

    public bool Formatted { get; set; } = true;


    public object? GetNewValue(Models.ColumnSettingModel columnSetting, SqlDataReader dataReader)
    {
        int length = 9;

        string number = GenerateRandomNumber(length);

        if (this.Formatted)
        {
            number = number.Insert(startIndex: 3, "-");
            number = number.Insert(startIndex: 6, "-");

            return number;
        }
        else
        {
            return number;
        }
    }

    public bool IsRecommendedForColumnName(string columnName)
    {
        columnName = columnName.StripNonAlphaNumericChars();
        return columnName.Contains("SocialSecurityNumber", StringComparison.OrdinalIgnoreCase)
            || columnName.Contains("SSN", StringComparison.OrdinalIgnoreCase);
    }


    /// <summary>
    /// Create a random number as a string with a maximum length.
    /// </summary>
    /// <param name="length">Length of number</param>
    /// <returns>Generated string</returns>
    private string GenerateRandomNumber(int length)
    {
        if (length > 0)
        {
            var sb = new StringBuilder();

            var rnd = SeedRandom();
            for (int i = 0; i < length; i++)
            {
                sb.Append(rnd.Next(0, 9).ToString());
            }

            return sb.ToString();
        }

        return string.Empty;
    }

    private Random SeedRandom()
    {
        return new Random(Guid.NewGuid().GetHashCode());
    }
}