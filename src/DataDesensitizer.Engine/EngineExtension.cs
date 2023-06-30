using DataDesensitizer.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace DataDesensitizer;

public static partial class EngineExtension
{
    public static void AddDataDesensitizerEngine(this IServiceCollection services)
    {
        services.AddSingleton<Abstractions.IProfileProcessor, ProfileProcessor>();
    }
}
