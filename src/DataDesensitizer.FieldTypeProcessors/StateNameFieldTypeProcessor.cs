using Microsoft.Extensions.Logging;

namespace DataDesensitizer.FieldTypeProcessors;
public class StateNameFieldTypeProcessor : SourceFileFieldTypeProcessorBase, Abstractions.IFieldTypeProcessor, IDisposable
{
    private readonly ILogger<StateNameFieldTypeProcessor> _logger;

    public StateNameFieldTypeProcessor(FieldTypeProcessorSettings settings, ILogger<StateNameFieldTypeProcessor> logger)
        : base(settings, logger)
    {
        _logger = logger;
    }

    public string Name => "State";
    protected override string Filename => "StateNames.txt";

    public bool IsRecommendedForColumnName(string columnName)
    {
        columnName = columnName.StripNonAlphaNumericChars();

        //State is tricky, because a lot of columns will use the word state for defining "condition", like the "EnrollmentState" is "Enrolled"
        return columnName.Contains("StateOrProvince", StringComparison.OrdinalIgnoreCase)
            || columnName.Equals("State", StringComparison.OrdinalIgnoreCase);
    }
}
