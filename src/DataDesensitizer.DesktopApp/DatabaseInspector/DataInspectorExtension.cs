using DataDesensitizer.DesktopApp.DatabaseInspector;
using DataDesensitizer.DesktopApp.DatabaseInspector.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DataDesensitizer;

public static partial class DatabaseInspectorExtension
{
    public static void AddDataDesensitizerDatabaseInspector(this IServiceCollection services)
    {
        services.AddSingleton<ITableRepository, TableRepository>();
        services.AddSingleton<IColumnRepository, ColumnRepository>();
    }
}
