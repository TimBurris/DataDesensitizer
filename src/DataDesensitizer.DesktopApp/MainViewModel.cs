using DataDesensitizer.DesktopApp.Flyouts.ViewModels;
using DataDesensitizer.DesktopApp.Panels.ViewModels;
using DataDesensitizer.DesktopApp.ToastNotification.Abstractions;
using System.Threading;

namespace DataDesensitizer.DesktopApp;
public class MainViewModel : NinjaMvvm.Wpf.WpfViewModelBase
{
    private readonly BuyMeACoffeeFlyoutViewModel _buyMeACoffeeFlyoutViewModel;
    private readonly IToastFaultlessExecutionService _toastFaultlessExecutionService;
    private readonly SelectDatabaseConnectionFlyoutViewModel _selectDatabaseConnectionFlyoutViewModel;
    private readonly ProfileViewModel _profileViewModel;

    public MainViewModel(
        Flyouts.ViewModels.BuyMeACoffeeFlyoutViewModel buyMeACoffeeFlyoutViewModel,
        ToastNotification.Abstractions.IToastFaultlessExecutionService toastFaultlessExecutionService,
        SelectDatabaseConnectionFlyoutViewModel selectDatabaseConnectionFlyoutViewModel,
        Panels.ViewModels.ProfileViewModel profileViewModel)
    {
        _buyMeACoffeeFlyoutViewModel = buyMeACoffeeFlyoutViewModel;
        _toastFaultlessExecutionService = toastFaultlessExecutionService;
        _selectDatabaseConnectionFlyoutViewModel = selectDatabaseConnectionFlyoutViewModel;
        _profileViewModel = profileViewModel;
    }

    protected override Task<bool> OnReloadDataAsync(CancellationToken cancellationToken)
    {
        if (!this.HasEverBeenLoaded)
        {
            _selectDatabaseConnectionFlyoutViewModel.FlyoutConfirmed += this._selectDatabaseConnectionFlyoutViewModel_FlyoutConfirmed;
            this.ShowFlyoutCommand.Execute(_selectDatabaseConnectionFlyoutViewModel);
        }

        return base.OnReloadDataAsync(cancellationToken);
    }

    private void _selectDatabaseConnectionFlyoutViewModel_FlyoutConfirmed(object? sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_selectDatabaseConnectionFlyoutViewModel.ConnectionString))
        {
            _profileViewModel.ConnectionString = _selectDatabaseConnectionFlyoutViewModel.ConnectionString;
            this.CurrentPanel = _profileViewModel;
        }
    }

    #region Binding Props

    public Flyouts.ViewModels.IFlyoutViewModel CurrentFlyout
    {
        get { return GetField<Flyouts.ViewModels.IFlyoutViewModel>(); }
        set { SetField(value); }
    }

    public bool IsFlyoutOpen
    {
        get { return GetField<bool>(); }
        set { SetField(value); }
    }


    public NinjaMvvm.Wpf.WpfViewModelBase CurrentPanel
    {
        get { return GetField<NinjaMvvm.Wpf.WpfViewModelBase>(); }
        set { SetField(value); }
    }

    #endregion

    #region BuyMeACoffee Command

    public NinjaMvvm.Wpf.RelayCommand BuyMeACoffeeCommand => new NinjaMvvm.Wpf.RelayCommand((param) => _toastFaultlessExecutionService.TryExecute(() => this.BuyMeACoffee()));

    public void BuyMeACoffee()
    {
        this.ShowFlyout(_buyMeACoffeeFlyoutViewModel);
    }

    #endregion

    #region ShowFlyout Command

    public NinjaMvvm.Wpf.RelayCommand<Flyouts.ViewModels.IFlyoutViewModel> ShowFlyoutCommand => new NinjaMvvm.Wpf.RelayCommand<Flyouts.ViewModels.IFlyoutViewModel>((param) => _toastFaultlessExecutionService.TryExecute(() => this.ShowFlyout(param)));

    public void ShowFlyout(Flyouts.ViewModels.IFlyoutViewModel item)
    {
        item.HideFlyoutCommand = this.HideFlyoutCommand;

        this.CurrentFlyout = item;
        this.IsFlyoutOpen = true;
    }

    #endregion

    #region HideFlyout Command

    public NinjaMvvm.Wpf.RelayCommand HideFlyoutCommand => new NinjaMvvm.Wpf.RelayCommand((param) => _toastFaultlessExecutionService.TryExecute(() => this.HideFlyout()));

    public void HideFlyout()
    {
        this.IsFlyoutOpen = false;
    }

    #endregion

    #region ShowConfirmationDialog Command

    public NinjaMvvm.Wpf.RelayCommand<ConfirmationDialogModel> ShowConfirmationDialogCommand => new NinjaMvvm.Wpf.RelayCommand<ConfirmationDialogModel>((param) => _toastFaultlessExecutionService.TryExecuteAsync(() => this.ShowConfirmationDialogAsync(param)));

    public async Task ShowConfirmationDialogAsync(ConfirmationDialogModel item)
    {

        var mySettings = new MahApps.Metro.Controls.Dialogs.MetroDialogSettings
        {
            AffirmativeButtonText = item.AffirmativeButtonText ?? "Yes",
            NegativeButtonText = item.NegativeButtonText ?? "Cancel",
            AnimateShow = true,
            AnimateHide = false,

        };

        var d = MahApps.Metro.Controls.Dialogs.DialogCoordinator.Instance;
        var result = await d.ShowMessageAsync(context: this,
             title: item.Title,
             message: item.Message,
             MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative,
             mySettings
            );

        item.OnComplete?.Invoke((result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative));
    }

    #endregion
}
