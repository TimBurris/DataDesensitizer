using Microsoft.Extensions.Logging;

namespace DataDesensitizer.FieldTypeProcessors;
public class CompanyNameFieldTypeProcessor : SourceFileFieldTypeProcessorBase, Abstractions.IFieldTypeProcessor, IDisposable
{
    private readonly ILogger<CompanyNameFieldTypeProcessor> _logger;

    public CompanyNameFieldTypeProcessor(FieldTypeProcessorSettings settings, ILogger<CompanyNameFieldTypeProcessor> logger)
        : base(settings, logger)
    {
        _logger = logger;
    }

    public string Name => "Company";

    protected override string Filename => "CompanyNames.txt";

    public bool IsRecommendedForColumnName(string columnName)
    {
        columnName = columnName.StripNonAlphaNumericChars();
        return columnName.Contains("CompanyName", StringComparison.OrdinalIgnoreCase)
            || columnName.Equals("Company", StringComparison.OrdinalIgnoreCase);//i don't want Contains("Company") because that'll hit things like CompanyEmailAddress
    }
}
