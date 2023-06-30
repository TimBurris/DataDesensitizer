using Microsoft.Extensions.Logging;

namespace DataDesensitizer.FieldTypeProcessors;
public class FirstNameFieldTypeProcessor : SourceFileFieldTypeProcessorBase, Abstractions.IFieldTypeProcessor, IDisposable
{
    private readonly ILogger<FirstNameFieldTypeProcessor> _logger;

    public FirstNameFieldTypeProcessor(FieldTypeProcessorSettings settings, ILogger<FirstNameFieldTypeProcessor> logger)
        : base(settings, logger)
    {
        _logger = logger;
    }

    public string Name => "First Name";
    protected override string Filename => "FirstNames.txt";
}
