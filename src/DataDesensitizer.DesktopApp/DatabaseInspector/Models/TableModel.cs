namespace DataDesensitizer.DesktopApp.DatabaseInspector.Models;

public record TableModel
{
    public TableModel(string schemaName, string tableName)
    {
        this.SchemaName = schemaName;
        this.TableName = tableName;
    }
    public string SchemaName { get; set; }
    public string TableName { get; set; }
}
