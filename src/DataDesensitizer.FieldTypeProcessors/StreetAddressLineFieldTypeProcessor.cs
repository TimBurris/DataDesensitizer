using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DataDesensitizer.FieldTypeProcessors;
public class StreetAddressLineFieldTypeProcessor : SourceFileFieldTypeProcessorBase, Abstractions.IFieldTypeProcessor, IDisposable
{
    private readonly ILogger<StreetAddressLineFieldTypeProcessor> _logger;

    public StreetAddressLineFieldTypeProcessor(FieldTypeProcessorSettings settings, ILogger<StreetAddressLineFieldTypeProcessor> logger)
        : base(settings, logger)
    {
        _logger = logger;
    }

    public string Name => "Street Address Line";
    protected override string Filename => "StreetNames.txt";

    public override object? GetNewValue(Models.ColumnSettingModel columnSetting, SqlDataReader dataReader)
    {
        //use the base File processor to get a street name, then we'll randomly add a street number
        var streetName = base.GetNewValue(columnSetting, dataReader);
        var streetNumber = this.GenerateRandomNumber();

        return streetNumber + " " + streetName;
    }

    public bool IsRecommendedForColumnName(string columnName)
    {
        columnName = columnName.StripNonAlphaNumericChars();
        return columnName.Contains("Street", StringComparison.OrdinalIgnoreCase)
            || columnName.Contains("AddressLine1", StringComparison.OrdinalIgnoreCase);
    }

    private string GenerateRandomNumber()
    {
        var rnd = SeedRandom();
        return rnd.Next(1, 99999).ToString();
    }

    private Random SeedRandom()
    {
        return new Random(Guid.NewGuid().GetHashCode());
    }
}
