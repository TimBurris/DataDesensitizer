using DataDesensitizer.FieldTypeProcessors;
using Microsoft.Extensions.DependencyInjection;

namespace DataDesensitizer;

public static partial class FieldTypeProcessorsExtension
{
    public static void AddFieldTypeProcessors(this IServiceCollection services, Action<Configuration> config)
    {
        var configuration = new Configuration()
        {
            //we'd apply any defaults here
        };

        //now after defaults applied, invoke
        config.Invoke(configuration);

        //now, using configuration, register our Settings class for injection
        var settings = new FieldTypeProcessorSettings(dataFilesPath: configuration.DataFilesPath);
        services.AddSingleton<FieldTypeProcessorSettings>(settings);


        services.AddScoped<Abstractions.IFieldTypeProcessorProvider, FieldTypeProcessorProvider>();

        //Register all our FieldTypeProcessors.  in the future this might be a "plug-in" architecture
        //these need to be transient so that each different column has their own different settings
        //   if the process needs to enforce uniqueness, it must use static variables or whatever it needs to ensure uniqueness
        services.AddTransient<PhoneNumberFieldTypeProcessor>();
        services.AddTransient<SalaryFieldTypeProcessor>();

        //for not these are singleton until we figure how we want to handle knowing when to dispose/close the base file
        services.AddSingleton<CompanyNameFieldTypeProcessor>();
        services.AddSingleton<FullNameFieldTypeProcessor>();
        services.AddSingleton<FirstNameFieldTypeProcessor>();
        services.AddSingleton<LastNameFieldTypeProcessor>();
        services.AddSingleton<EmailAddressFieldTypeProcessor>();
        services.AddSingleton<StreetAddressLineFieldTypeProcessor>();
        services.AddSingleton<CityNameFieldTypeProcessor>();
        services.AddSingleton<StateAbbreviationFieldTypeProcessor>();
        services.AddSingleton<StateNameFieldTypeProcessor>();
        services.AddSingleton<ZipCodeFieldTypeProcessor>();


    }


    public class Configuration
    {
        public string DataFilesPath { get; set; } = ".\\";
    }
}
