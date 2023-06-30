using DataDesensitizer.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DataDesensitizer.Engine;

public class ProfileProcessor : Abstractions.IProfileProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProfileProcessor> _logger;

    public ProfileProcessor(IServiceProvider serviceProvider, ILogger<ProfileProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public void RunProfile(ProfileModel profile, string connectionString)
    {
        string json = System.Text.Json.JsonSerializer.Serialize(profile);

        _logger.LogDebug("running {profile} against {connectionString}", json, connectionString);
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            foreach (var t in profile.TableSettings)
            {
                this.ProcessTable(connection, t);
            }

            connection.Close();
        }
    }

    private void LogCommand(SqlCommand command)
    {
        try
        {
            var parameters = command.Parameters.Cast<IDataParameter>().ToList();
            object?[] sqlArgValues = parameters.Select(x => x?.Value).ToArray();

            //this try catch is because i've seen some weird instances where the sqlArgValues fail to log, so if that happens, we'll re-log without the values
            try
            {
                _logger.LogDebug("Executing Sql:\r\n{sql}\r\n with args:\r\n{sqlArg}", command.CommandText, sqlArgValues);
            }
            catch
            {
                _logger.LogDebug("Executing Sql:\r\n{sql}\r\n with args:\r\n-args failed to log-", command.CommandText);
            }
        }
        catch { }//i really don't think this will happen, but i don't want issues with logging to break things
    }

    private SqlDataReader ExecuteReadCommand(SqlCommand command)
    {
        this.LogCommand(command);

        return command.ExecuteReader();
    }

    private void ExecuteUpdateCommand(SqlCommand updateCommand)
    {
        this.LogCommand(updateCommand);
        updateCommand.ExecuteNonQuery();
    }

    private void ProcessTable(SqlConnection connection, TableSettingModel t)
    {
        var primaryKeyColumns = this.GetPrimaryKeyColumnNames(connection, t);

        List<string> columnsToPull = t.ColumnSettings.Select(x => x.ColumnName)
            .Union(primaryKeyColumns)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var sb = new System.Text.StringBuilder();
        sb.Append("SELECT ");
        bool first = true;

        foreach (var columnName in columnsToPull)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                sb.Append(',');
            }

            sb.Append($"[{columnName}]");
        }

        sb.Append($" FROM [{t.SchemaName}].[{t.TableName}]");

        if (t.Randomize)
        {
            sb.Append(" ORDER BY newid()");//randomize by sorting and a new guid for every row
        }


        Dictionary<ColumnSettingModel, Abstractions.IFieldTypeProcessor> fieldProcessorsByColumnSetting = new();
        foreach (var columnSetting in t.ColumnSettings)
        {
            //TODO: we should create the services based on if they are unique by table or db and dispose at that scope
            var type = Type.GetType(columnSetting.FieldTypeProcessorTypeName);
            if (type == null)
            {
                throw new ApplicationException($"Type {columnSetting.FieldTypeProcessorTypeName} was not found");
            }
            var processor = _serviceProvider.GetService(type) as Abstractions.IFieldTypeProcessor;
            if (processor == null)
            {
                throw new ApplicationException($"{columnSetting.FieldTypeProcessorTypeName} was not registered or does not implement IFieldTypeProcessor");
            }
            fieldProcessorsByColumnSetting.Add(columnSetting, processor);
        }

        string selectSql = sb.ToString();
        using (var command = new SqlCommand(selectSql, connection))
        using (var dataReader = this.ExecuteReadCommand(command))
        {
            while (dataReader.Read())
            {
                this.ProcessRow(dataReader, connection, t, primaryKeyColumns, fieldProcessorsByColumnSetting);
            }
        }

        //TODO: only dispose field processors that are not db unique
        //dispose field processors 
        foreach (var processor in fieldProcessorsByColumnSetting.Values)
        {
            if (processor is IDisposable)
            {
                ((IDisposable)processor).Dispose();
            }
        }
    }

    private List<string> GetPrimaryKeyColumnNames(SqlConnection connection, TableSettingModel t)
    {
        var results = new List<string>();
        var sql = @"SELECT COLUMN_NAME
                    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                    INNER JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu
                            ON tc.CONSTRAINT_NAME = ccu.Constraint_name
                    WHERE tc.CONSTRAINT_TYPE = 'Primary Key'
                        AND tc.TABLE_SCHEMA = @0
                        AND tc.TABLE_NAME = @1";

        using (var command = new SqlCommand(sql, connection))
        {
            command.Parameters.Add(new SqlParameter(parameterName: "@0", value: t.SchemaName));
            command.Parameters.Add(new SqlParameter(parameterName: "@1", value: t.TableName));

            using (SqlDataReader dataReader = this.ExecuteReadCommand(command))
            {

                while (dataReader.Read())
                {
                    results.Add(dataReader.GetString(0));
                }
            }
        }

        return results;
    }

    private void ProcessRow(SqlDataReader dataReader, SqlConnection connection, TableSettingModel t, IEnumerable<string> primaryKeyColumns, Dictionary<ColumnSettingModel, Abstractions.IFieldTypeProcessor> fieldProcessorsByColumnSetting)
    {
        using (SqlCommand updateCommand = new SqlCommand())
        {

            System.Text.StringBuilder sb = new();
            sb.Append($"Update [{t.SchemaName}].[{t.TableName}] SET ");

            int i = 0;
            foreach (var columnSetting in t.ColumnSettings)
            {
                var processor = fieldProcessorsByColumnSetting[columnSetting];

                object? value = processor.GetNewValue(columnSetting, dataReader);
                string paramName = $"@{i}";

                if (i > 0)
                {
                    sb.Append(',');
                }
                sb.Append($" [{columnSetting.ColumnName}] = {paramName}");
                updateCommand.Parameters.Add(new SqlParameter(parameterName: paramName, value: value));
                i++;
            }

            //build the where clause using each of the Primary key columns
            sb.Append(" WHERE ");

            bool firstPK = true;
            foreach (var pk in primaryKeyColumns)
            {
                if (firstPK)
                {
                    firstPK = false;
                }
                else
                {
                    sb.Append(" AND ");
                }

                string paramName = $"@{i}";
                sb.Append($"[{pk}] = {paramName}");

                object value = dataReader.GetValue(pk);//use DataReader here because we want the ORIGINAL value, because it's possible our PK is part of what is being updated, so the value in the SET would be different than what's here in the WHERE
                updateCommand.Parameters.Add(new SqlParameter(parameterName: paramName, value: value));
                i++;
            }

            updateCommand.Connection = connection;
            updateCommand.CommandText = sb.ToString();

            this.ExecuteUpdateCommand(updateCommand);

        }
    }

}
