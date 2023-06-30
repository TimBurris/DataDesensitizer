using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Text;

namespace DataDesensitizer.FieldTypeProcessors;

public class ZipCodeFieldTypeProcessor : Abstractions.IFieldTypeProcessor
{
    private readonly ILogger<ZipCodeFieldTypeProcessor> _logger;

    public ZipCodeFieldTypeProcessor(FieldTypeProcessorSettings settings, ILogger<ZipCodeFieldTypeProcessor> logger)
    {
        _logger = logger;
    }

    public string Name => "Zip Code";

    public object? GetNewValue(Models.ColumnSettingModel columnSetting, SqlDataReader dataReader)
    {
        return this.GenerateRandomNumber(length: 5);
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