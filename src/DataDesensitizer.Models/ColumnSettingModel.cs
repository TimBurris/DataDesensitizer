namespace DataDesensitizer.Models;

public class ColumnSettingModel
{
    public enum UniquenessOption
    {
        Unique,
        PreferUnique,
        NotUnique,
    }

    public ColumnSettingModel(string columnName, string fieldTypeProcessorTypeName)
    {
        this.ColumnName = columnName;
        this.FieldTypeProcessorTypeName = fieldTypeProcessorTypeName;
    }

    /// <summary>
    /// when assigning a value, how unique should it be within the table
    /// </summary>
  //TODO:  public UniquenessOption TableUniqueness { get; set; }

    /// <summary>
    /// when assigning a value, how unique should it be within the database
    /// </summary>
  //TODO:  public UniquenessOption DatabaseUniqueness { get; set; }

    public string ColumnName { get; set; }

    public string FieldTypeProcessorTypeName { get; set; }

}
