namespace DataDesensitizer.Models;

public class ProfileModel
{
    public ProfileModel(string profileName)
    {
        this.ProfileName = profileName;
    }

    public string ProfileName { get; set; }
    public List<TableSettingModel> TableSettings { get; set; } = new List<TableSettingModel>();
}
