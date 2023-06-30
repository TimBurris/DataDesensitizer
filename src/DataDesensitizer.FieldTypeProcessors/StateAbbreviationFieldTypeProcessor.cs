using Microsoft.Extensions.Logging;

namespace DataDesensitizer.FieldTypeProcessors;
public class StateAbbreviationFieldTypeProcessor : SourceFileFieldTypeProcessorBase, Abstractions.IFieldTypeProcessor, IDisposable
{
    private readonly ILogger<StateAbbreviationFieldTypeProcessor> _logger;

    public StateAbbreviationFieldTypeProcessor(FieldTypeProcessorSettings settings, ILogger<StateAbbreviationFieldTypeProcessor> logger)
        : base(settings, logger)
    {
        _logger = logger;
    }

    public string Name => "State Abbreviation";
    protected override string Filename => "StateAbbreviations.txt";

    public bool IsRecommendedForColumnName(string columnName)
    {
        columnName = columnName.StripNonAlphaNumericChars();
        //If we passed in the column length we could do a better job here, cuz we'd expect something like char(2)

        //State is tricky, because a lot of columns will use the word state for defining "condition", like the "EnrollmentState" is "Enrolled"
        return columnName.Contains("StateOrProvince", StringComparison.OrdinalIgnoreCase)
            || columnName.Equals("State", StringComparison.OrdinalIgnoreCase);
    }
}
