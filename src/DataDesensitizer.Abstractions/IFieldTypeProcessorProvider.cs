namespace DataDesensitizer.Abstractions;
public interface IFieldTypeProcessorProvider
{
    IEnumerable<Abstractions.IFieldTypeProcessor> GetAll();
    Abstractions.IFieldTypeProcessor? Get(string fieldTypeProcessorTypeName);
}
