using Microsoft.Data.SqlClient;
using System.Data;

namespace DataDesensitizer.DesktopApp.DatabaseInspector;

public class ColumnRepository : Abstractions.IColumnRepository
{
    public IEnumerable<Models.ColumnModel> GetAllForTable(string schemaName, string tableName, string connectionString)
    {
        var results = new List<Models.ColumnModel>();
        var sql = @"SELECT COLUMN_NAME 
                    FROM INFORMATION_SCHEMA.COLUMNS c 
                    WHERE TABLE_SCHEMA=@0 AND TABLE_NAME=@1";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add(new SqlParameter(parameterName: "@0", value: schemaName));
                command.Parameters.Add(new SqlParameter(parameterName: "@1", value: tableName));

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        string columnName = dataReader.GetString("COLUMN_NAME");
                        results.Add(new Models.ColumnModel(columnName));
                    }
                }
            }
            connection.Close();
        }
        return results;
    }
}
