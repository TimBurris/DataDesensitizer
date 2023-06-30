using DataDesensitizer.Models;

namespace DataDesensitizer.Abstractions;

public interface IProfileProcessor
{
    void RunProfile(ProfileModel profile, string connectionString);
}