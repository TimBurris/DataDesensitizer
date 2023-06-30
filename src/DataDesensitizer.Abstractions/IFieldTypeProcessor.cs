using Microsoft.Data.SqlClient;

namespace DataDesensitizer.Abstractions;

public interface IFieldTypeProcessor
{
    string Name { get; }
    //  object Settings { get; set; }// how would this work in a UI? is this just going too far?  probably out of scope for MVP

    object? GetNewValue(Models.ColumnSettingModel columnSetting, SqlDataReader dataReader);


}
