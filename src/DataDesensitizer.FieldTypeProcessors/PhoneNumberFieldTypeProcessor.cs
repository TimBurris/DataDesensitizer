using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Text;

namespace DataDesensitizer.FieldTypeProcessors;

public class PhoneNumberFieldTypeProcessor : Abstractions.IFieldTypeProcessor
{
    private readonly ILogger<PhoneNumberFieldTypeProcessor> _logger;

    public PhoneNumberFieldTypeProcessor(FieldTypeProcessorSettings settings, ILogger<PhoneNumberFieldTypeProcessor> logger)
    {
        _logger = logger;
    }

    public string Name => "Phone Number";

    public bool Formatted { get; set; } = true;
    public bool Use555 { get; set; } = true;

    //TODO: support Formats like "None" or standard or hyphened, etc maybe some type of "template" {country} ({areacode}) {whaterver}-{whateverx}
    //TODO: maybe allow them to specify they all have 555 to ensure no "real" number?  that could be handled by the template, right?

    public object? GetNewValue(Models.ColumnSettingModel columnSetting, SqlDataReader dataReader)
    {
        return this.GenerateRandomPhoneNumber();
    }

    private string GenerateRandomPhoneNumber()
    {
        int length = this.Use555 ? 7 : 10;

        string number = GenerateRandomNumber(length);
        if (this.Use555)
        {
            number = number.Insert(startIndex: 3, value: "555");
        }

        if (this.Formatted)
        {
            string areaCode = number.Substring(startIndex: 0, length: 3);
            string exchange = number.Substring(startIndex: 3, length: 3);
            string extension = number.Substring(startIndex: 6, length: 4);

            return $"({areaCode}) {exchange}-{extension}";
        }
        else
        {
            return number;
        }
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