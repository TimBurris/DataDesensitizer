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
}
