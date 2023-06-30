using Microsoft.Extensions.Logging;

namespace DataDesensitizer.FieldTypeProcessors;
public class LastNameFieldTypeProcessor : SourceFileFieldTypeProcessorBase, Abstractions.IFieldTypeProcessor, IDisposable
{
    private readonly ILogger<LastNameFieldTypeProcessor> _logger;

    public LastNameFieldTypeProcessor(FieldTypeProcessorSettings settings, ILogger<LastNameFieldTypeProcessor> logger)
        : base(settings, logger)
    {
        _logger = logger;
    }

    public string Name => "Last Name";
    protected override string Filename => "LastNames.txt";
}
