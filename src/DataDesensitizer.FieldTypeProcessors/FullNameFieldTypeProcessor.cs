using Microsoft.Extensions.Logging;

namespace DataDesensitizer.FieldTypeProcessors;
public class FullNameFieldTypeProcessor : SourceFileFieldTypeProcessorBase, Abstractions.IFieldTypeProcessor, IDisposable
{
    private readonly ILogger<FullNameFieldTypeProcessor> _logger;

    public FullNameFieldTypeProcessor(FieldTypeProcessorSettings settings, ILogger<FullNameFieldTypeProcessor> logger)
        : base(settings, logger)
    {
        _logger = logger;
    }

    public string Name => "Full Name";

    protected override string Filename => "FullNames.txt";
}
