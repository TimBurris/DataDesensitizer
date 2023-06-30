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
}
