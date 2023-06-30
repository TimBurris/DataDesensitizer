namespace DataDesensitizer.DesktopApp.DatabaseInspector.Models;

public record ColumnModel
{
    public ColumnModel(string columnName)
    {
        this.ColumnName = columnName;
    }
    public string ColumnName { get; set; }
}
