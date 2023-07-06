using Microsoft.Extensions.Logging;

namespace DataDesensitizer.FieldTypeProcessors;

public class SalaryFieldTypeProcessor : Abstractions.IFieldTypeProcessor
{
    private readonly ILogger<SalaryFieldTypeProcessor> _logger;

    public SalaryFieldTypeProcessor(FieldTypeProcessorSettings settings, ILogger<SalaryFieldTypeProcessor> logger)
    {
        _logger = logger;
    }

    public string Name => "Salary";

    //Future:
    //public int Minimum { get; set; } = 50000;
    //public int Maximum { get; set; } = 500000;
    //public bool RoundToNearestThousand { get; set; } = true;

    public object? GetNewValue(Models.ColumnSettingModel columnSetting)
    {
        return this.GenerateRandomSalary();
    }

    public bool IsRecommendedForColumnName(string columnName)
    {
        columnName = columnName.StripNonAlphaNumericChars();
        return columnName.Contains("Salary", StringComparison.OrdinalIgnoreCase);
    }

    private int GenerateRandomSalary()
    {
        var rnd = SeedRandom();
        int value = rnd.Next(minValue: 20, 500);//20k/500k
        return value * 1000;//a number in thousands of dollars
    }

    private Random SeedRandom()
    {
        return new Random(Guid.NewGuid().GetHashCode());
    }
}