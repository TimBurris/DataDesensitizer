namespace DataDesensitizer.DesktopApp.DatabaseInspector.Abstractions;

public interface IColumnRepository
{
    IEnumerable<Models.ColumnModel> GetAllForTable(string schemaName, string tableName, string connectionString);
}
