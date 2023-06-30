using Microsoft.Data.SqlClient;
using System.Data;

namespace DataDesensitizer.DesktopApp.DatabaseInspector;

public class TableRepository : Abstractions.ITableRepository
{
    public IEnumerable<Models.TableModel> GetAll(string connectionString)
    {
        var results = new List<Models.TableModel>();
        var sql = @"SELECT TABLE_SCHEMA, TABLE_NAME
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_TYPE='BASE TABLE'";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqlCommand(sql, connection))
            {
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        string schemaName = dataReader.GetString("TABLE_SCHEMA");
                        string tableName = dataReader.GetString("TABLE_NAME");
                        results.Add(new Models.TableModel(schemaName: schemaName, tableName: tableName));
                    }
                }
            }
            connection.Close();
        }
        return results;
    }
}
