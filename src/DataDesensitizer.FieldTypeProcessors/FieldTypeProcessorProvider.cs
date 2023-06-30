using Microsoft.Extensions.DependencyInjection;

namespace DataDesensitizer.FieldTypeProcessors;

public class FieldTypeProcessorProvider : Abstractions.IFieldTypeProcessorProvider
{
    private readonly IServiceProvider _serviceProvider;

    public FieldTypeProcessorProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IEnumerable<Abstractions.IFieldTypeProcessor> GetAll()
    {
        var types = this.GetType().Assembly.GetTypes().Where(x => x.GetInterface(nameof(Abstractions.IFieldTypeProcessor)) != null);

        foreach (var type in types)
        {
            yield return (Abstractions.IFieldTypeProcessor)_serviceProvider.GetRequiredService(type);
        }
    }

    public Abstractions.IFieldTypeProcessor? Get(string fieldTypeProcessorTypeName)
    {
        var type = Type.GetType(fieldTypeProcessorTypeName);
        if (type == null)
        {
            return null;
        }

        var processor = _serviceProvider.GetService(type) as Abstractions.IFieldTypeProcessor;

        return processor;
    }
}