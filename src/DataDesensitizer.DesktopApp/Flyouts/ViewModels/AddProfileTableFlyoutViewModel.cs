using DataDesensitizer.DesktopApp.ToastNotification.Abstractions;
using System.Windows.Input;

namespace DataDesensitizer.DesktopApp.Flyouts.ViewModels;
public class AddProfileTableFlyoutViewModel : NinjaMvvm.Wpf.WpfViewModelBase, IFlyoutViewModel
{
    private readonly IToastFaultlessExecutionService _toastFaultlessExecutionService;

    public event EventHandler? FlyoutConfirmed;
    public ICommand? HideFlyoutCommand { get; set; }
    public override string ViewTitle => "Table Setup";
    public AddProfileTableFlyoutViewModel(ToastNotification.Abstractions.IToastFaultlessExecutionService toastFaultlessExecutionService)
    {
        _toastFaultlessExecutionService = toastFaultlessExecutionService;
    }

    public void Reset()
    {
        this.SchemaName = null;
        this.TableName = null;
    }


    public string? SchemaName
    {
        get { return GetField<string?>(); }
        set { SetField(value); }
    }

    public string? TableName
    {
        get { return GetField<string?>(); }
        set { SetField(value); }
    }

    #region Okay Command

    public NinjaMvvm.Wpf.RelayCommand OkayCommand => new NinjaMvvm.Wpf.RelayCommand((param) => _toastFaultlessExecutionService.TryExecute(() => this.Okay()));

    public void Okay()
    {


        this.FlyoutConfirmed?.Invoke(this, EventArgs.Empty);

        this.HideFlyoutCommand?.Execute(parameter: null);
    }

    #endregion

}
