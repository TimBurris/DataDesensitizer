using Microsoft.Extensions.Logging;

namespace DataDesensitizer.FieldTypeProcessors;

public class EmailAddressFieldTypeProcessor : SourceFileFieldTypeProcessorBase, Abstractions.IFieldTypeProcessor, IDisposable
{
    private readonly ILogger<EmailAddressFieldTypeProcessor> _logger;

    public EmailAddressFieldTypeProcessor(FieldTypeProcessorSettings settings, ILogger<EmailAddressFieldTypeProcessor> logger)
        : base(settings, logger)
    {
        _logger = logger;
    }

    public string Name => "E-Mail Address";

    //alternatively we could just have a list of domains and generate randoms
    protected override string Filename => "EmailAddresses.txt";


    public bool IsRecommendedForColumnName(string columnName)
    {
        columnName = columnName.StripNonAlphaNumericChars();
        return columnName.Contains("EmailAddress", StringComparison.OrdinalIgnoreCase)
            || columnName.EndsWith("Email", StringComparison.OrdinalIgnoreCase);
    }
}
