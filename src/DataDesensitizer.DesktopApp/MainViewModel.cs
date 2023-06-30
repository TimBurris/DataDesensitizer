using DataDesensitizer.Abstractions;
using DataDesensitizer.DesktopApp.Flyouts.ViewModels;
using DataDesensitizer.DesktopApp.Panels.ViewModels;
using DataDesensitizer.DesktopApp.ToastNotification.Abstractions;
using System.Threading;

namespace DataDesensitizer.DesktopApp;
public class MainViewModel : NinjaMvvm.Wpf.WpfViewModelBase
{
    private readonly BuyMeACoffeeFlyoutViewModel _buyMeACoffeeFlyoutViewModel;
    private readonly IToastFaultlessExecutionService _toastFaultlessExecutionService;
    private readonly IToastNotificationService _toastNotificationService;
    private readonly IProfileProcessor _profileProcessor;
    private readonly ProfileViewModel _profileViewModel;

    public MainViewModel(
        Flyouts.ViewModels.BuyMeACoffeeFlyoutViewModel buyMeACoffeeFlyoutViewModel,
        ToastNotification.Abstractions.IToastFaultlessExecutionService toastFaultlessExecutionService,
        ToastNotification.Abstractions.IToastNotificationService toastNotificationService,
        Abstractions.IProfileProcessor profileProcessor,
        Panels.ViewModels.ProfileViewModel profileViewModel)
    {
        _buyMeACoffeeFlyoutViewModel = buyMeACoffeeFlyoutViewModel;
        _toastFaultlessExecutionService = toastFaultlessExecutionService;
        _toastNotificationService = toastNotificationService;
        _profileProcessor = profileProcessor;
        _profileViewModel = profileViewModel;
    }

    protected override Task<bool> OnReloadDataAsync(CancellationToken cancellationToken)
    {
        this.ConnectionString = "Data Source=(local);Initial Catalog=SeminarEdge;Trusted_Connection=true;MultipleActiveResultSets=true;encrypt=false;";

        var content = System.IO.File.ReadAllText(@"E:\Temp\DataDesensitizer\SeminarEdgeProfile.json");

        var profile = System.Text.Json.JsonSerializer.Deserialize<Models.ProfileModel>(content);
        _profileViewModel.AssignProfile(profile);

        this.CurrentPanel = _profileViewModel;


        return base.OnReloadDataAsync(cancellationToken);
    }

    #region Binding Props

    public string ConnectionString
    {
        get { return GetField<string>(); }
        set { SetField(value); }
    }

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

    #region RunProcessor Command

    public NinjaMvvm.Wpf.RelayCommand RunProcessorCommand => new NinjaMvvm.Wpf.RelayCommand((param) => _toastFaultlessExecutionService.TryExecuteAsync(() => this.RunProcessorAsync()));

    public async Task RunProcessorAsync()
    {
        this.IsBusy = true;

        try
        {
            var content = System.IO.File.ReadAllText(@"E:\Temp\DataDesensitizer\SeminarEdgeProfile.json");

            var profile = System.Text.Json.JsonSerializer.Deserialize<Models.ProfileModel>(content);
            if (profile == null)
            {
                _toastNotificationService.ShowToast(ToastNotification.ToastType.Warning, "Profile empty");
                return;
            }
            //var profile = new ProfileModel(profileName: "")
            //{
            //    TableSettings = new List<TableSettingModel>()
            //    {
            //        new TableSettingModel(schemaName:"dbo", tableName:"AgentContact")
            //        {
            //            Randomize = true,
            //            ColumnSettings=new List<ColumnSettingModel>()
            //            {
            //                new ColumnSettingModel(columnName:"Name",typeof(FullNameFieldTypeProcessor).AssemblyQualifiedName),
            //                new ColumnSettingModel(columnName:"WorkPhoneNumber",typeof(PhoneNumberFieldTypeProcessor).AssemblyQualifiedName),
            //            }
            //        },

            //         new TableSettingModel(schemaName:"dbo", tableName:"Agent")
            //        {
            //            Randomize = true,
            //            ColumnSettings=new List<ColumnSettingModel>()
            //            {
            //                new ColumnSettingModel(columnName:"CompanyName",typeof(CompanyNameFieldTypeProcessor).AssemblyQualifiedName),
            //                new ColumnSettingModel(columnName:"AddressLine1",typeof(StreetAddressLineFieldTypeProcessor).AssemblyQualifiedName),
            //                new ColumnSettingModel(columnName:"City",typeof(CityNameFieldTypeProcessor).AssemblyQualifiedName),
            //                new ColumnSettingModel(columnName:"StateOrProvince",typeof(StateAbbreviationFieldTypeProcessor).AssemblyQualifiedName),
            //                new ColumnSettingModel(columnName:"ZipCode",typeof(ZipCodeFieldTypeProcessor).AssemblyQualifiedName),
            //            }
            //        }
            //    }
            //};


            _toastNotificationService.ShowToast(ToastNotification.ToastType.Info, "starting....");
            await Task.Run(() => _profileProcessor.RunProfile(profile, this.ConnectionString));
            _toastNotificationService.ShowToast(ToastNotification.ToastType.Success, "Completed Successfully!");
        }
        finally
        {
            this.IsBusy = false;
        }
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
}
