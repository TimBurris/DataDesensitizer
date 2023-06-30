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
}
