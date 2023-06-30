namespace DataDesensitizer.DesktopApp.DatabaseInspector.Abstractions;

public interface ITableRepository
{
    IEnumerable<Models.TableModel> GetAll(string connectionString);
}
