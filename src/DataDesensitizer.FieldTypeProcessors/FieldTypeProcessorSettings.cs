namespace DataDesensitizer;

public record FieldTypeProcessorSettings
{
    public FieldTypeProcessorSettings(string dataFilesPath)
    {
        this.DataFilesPath = dataFilesPath;
    }

    public string DataFilesPath { get; }
}