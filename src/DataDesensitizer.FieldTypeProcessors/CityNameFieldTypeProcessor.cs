using Microsoft.Extensions.Logging;

namespace DataDesensitizer.FieldTypeProcessors;
public class CityNameFieldTypeProcessor : SourceFileFieldTypeProcessorBase, Abstractions.IFieldTypeProcessor, IDisposable
{
    private readonly ILogger<CityNameFieldTypeProcessor> _logger;

    public CityNameFieldTypeProcessor(FieldTypeProcessorSettings settings, ILogger<CityNameFieldTypeProcessor> logger)
        : base(settings, logger)
    {
        _logger = logger;
    }

    public string Name => "City";
    protected override string Filename => "CityNames.txt";
}
