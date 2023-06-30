namespace DataDesensitizer.Models;

public class TableSettingModel
{
    public TableSettingModel(string schemaName, string tableName)
    {
        this.SchemaName = schemaName;
        this.TableName = tableName;
    }

    /// <summary>
    /// when true, randomizes the select so that sample results are applied in a random order
    /// </summary>
    public bool Randomize { get; set; }
    public string SchemaName { get; set; }
    public string TableName { get; set; }
    public List<ColumnSettingModel> ColumnSettings { get; set; } = new List<ColumnSettingModel>();
}
