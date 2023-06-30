using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace DataDesensitizer.DesktopApp;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    internal static IHost? AppHost { get; set; }

    public App()
    {
        AppHost = HostBuilder.Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        //AppHost will be null only if there was an error initializing
        if (AppHost == null)
        {
            this.Shutdown();
            return;
        }

        var logger = AppHost.Services.GetRequiredService<ILogger<App>>();

        try
        {
            SetupExceptionHandling(logger);

            await AppHost.StartAsync();

            logger.LogInformation("register data templates");
            RegisterViewToViewModelDataTemplates();

            logger.LogInformation("start form");

            var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
            startupForm.Show();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "you are a terrible developer");

            //   we must flush, otherwise the console might close out before logs are done writing
            Serilog.Log.CloseAndFlush();

            this.Shutdown();
        }
        base.OnStartup(e);
    }


    protected override void OnExit(ExitEventArgs e)
    {
        //   we must flush, otherwise the console might close out before logs are done writing
        Serilog.Log.CloseAndFlush();

        base.OnExit(e);
    }

    private void SetupExceptionHandling(ILogger logger)
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            logger.LogError((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

        DispatcherUnhandledException += (s, e) =>
        {
            logger.LogError(e.Exception, "Application.Current.DispatcherUnhandledException");
            e.Handled = true;
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            logger.LogError(e.Exception, "TaskScheduler.UnobservedTaskException");
            e.SetObserved();
        };

        logger.LogInformation("exception handling is setup");
    }

    private void RegisterViewToViewModelDataTemplates()
    {
        var manager = new DataTemplateManager();

        //flyouts
        manager.RegisterDataTemplate<Flyouts.ViewModels.BuyMeACoffeeFlyoutViewModel, Flyouts.Views.BuyMeACoffeeFlyoutView>();
        manager.RegisterDataTemplate<Flyouts.ViewModels.AddProfileTableFlyoutViewModel, Flyouts.Views.AddProfileTableFlyoutView>();

        //panels
        manager.RegisterDataTemplate<Panels.ViewModels.ProfileViewModel, Panels.Views.ProfileView>();
    }
}
