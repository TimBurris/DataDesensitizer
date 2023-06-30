using DataDesensitizer.DesktopApp.ToastNotification.Abstractions;
using System.Windows.Input;

namespace DataDesensitizer.DesktopApp.Flyouts.ViewModels;
public class BuyMeACoffeeFlyoutViewModel : NinjaMvvm.Wpf.WpfViewModelBase, IFlyoutViewModel
{
    private readonly IToastFaultlessExecutionService _toastFaultlessExecutionService;

    public event EventHandler? FlyoutConfirmed;
    public ICommand? HideFlyoutCommand { get; set; }

    public BuyMeACoffeeFlyoutViewModel(ToastNotification.Abstractions.IToastFaultlessExecutionService toastFaultlessExecutionService)
    {
        _toastFaultlessExecutionService = toastFaultlessExecutionService;
    }

    #region LaunchWebSite Command

    public NinjaMvvm.Wpf.RelayCommand LaunchWebSiteCommand => new NinjaMvvm.Wpf.RelayCommand((param) => _toastFaultlessExecutionService.TryExecute(() => this.LaunchWebSite()));

    public void LaunchWebSite()
    {
        string url = "https://www.buymeacoffee.com/timburris";

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });

        this.FlyoutConfirmed?.Invoke(this, EventArgs.Empty);//i don't think anyone will be listening, but if someone is listening, let them know the user chose wisely :p

        this.HideFlyoutCommand?.Execute(parameter: null);
    }

    #endregion

}
