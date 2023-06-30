
using DataDesensitizer.Abstractions;
using DataDesensitizer.DesktopApp.Flyouts.ViewModels;
using DataDesensitizer.DesktopApp.Navigator.Abstractions;
using DataDesensitizer.DesktopApp.ToastNotification.Abstractions;

namespace DataDesensitizer.DesktopApp.Panels.ViewModels;

public class ProfileViewModel : NinjaMvvm.Wpf.WpfViewModelBase
{
    private readonly IFieldTypeProcessorProvider _fieldTypeProcessorProvider;
    private readonly AddProfileTableFlyoutViewModel _addProfileTableFlyoutViewModel;
    private readonly INavigator _navigator;
    private readonly IToastFaultlessExecutionService _toastFaultlessExecutionService;
    private Models.ProfileModel? _profileModel;
    public ProfileViewModel(Abstractions.IFieldTypeProcessorProvider fieldTypeProcessorProvider,
        Flyouts.ViewModels.AddProfileTableFlyoutViewModel addProfileTableFlyoutViewModel,
        Navigator.Abstractions.INavigator navigator,
        ToastNotification.Abstractions.IToastFaultlessExecutionService toastFaultlessExecutionService)
    {
        _fieldTypeProcessorProvider = fieldTypeProcessorProvider;
        _addProfileTableFlyoutViewModel = addProfileTableFlyoutViewModel;
        _navigator = navigator;
        _toastFaultlessExecutionService = toastFaultlessExecutionService;
    }

    protected override void OnUnloaded()
    {
        base.OnUnloaded();
        _addProfileTableFlyoutViewModel.FlyoutConfirmed -= this._addProfileTableFlyoutViewModel_FlyoutConfirmed;
    }

    protected override void OnBound()
    {
        base.OnBound();
        _addProfileTableFlyoutViewModel.FlyoutConfirmed += this._addProfileTableFlyoutViewModel_FlyoutConfirmed;
    }

    private void _addProfileTableFlyoutViewModel_FlyoutConfirmed(object? sender, EventArgs e)
    {
        //when "confirmed" these really should never be null
        string schemaName = _addProfileTableFlyoutViewModel.SchemaName ?? "-not set-";
        string tableName = _addProfileTableFlyoutViewModel.TableName ?? "-not set-";

        var tableSettings = new Models.TableSettingModel(schemaName: schemaName, tableName: tableName);
        this.MenuLineItems.Add(new MenuLineItem(new TableLineItem(tableSettings, _fieldTypeProcessorProvider)));
    }

    public void AssignProfile(Models.ProfileModel profileModel)
    {
        _profileModel = profileModel;
        this.LoadMenuItems();
    }

    private void LoadMenuItems()
    {
        this.MenuLineItems.Clear();
        this.MenuLineItems.Add(new MenuLineItem()
        {
            IsAll = true,
            Name = "All"
        });

        if (_profileModel != null)
        {
            foreach (var t in _profileModel.TableSettings)
            {
                this.MenuLineItems.Add(new MenuLineItem(new TableLineItem(t, _fieldTypeProcessorProvider)));
            }
        }
    }


    #region AddTable Command

    public NinjaMvvm.Wpf.RelayCommand AddTableCommand => new NinjaMvvm.Wpf.RelayCommand((param) => _toastFaultlessExecutionService.TryExecute(() => this.AddTable()));

    public void AddTable()
    {
        _addProfileTableFlyoutViewModel.Reset();

        _navigator.ShowFlyout(_addProfileTableFlyoutViewModel);
    }

    #endregion


    #region AddColumn Command

    public NinjaMvvm.Wpf.RelayCommand<TableLineItem> AddColumnCommand => new NinjaMvvm.Wpf.RelayCommand<TableLineItem>((param) => _toastFaultlessExecutionService.TryExecute(() => this.AddColumn(param)));

    public void AddColumn(TableLineItem item)
    {
        //show flyout to create new item
        //item.ColumnLineItems.Add(new ColumnLineItem()
        //{
        //    FieldType = "-Not Set -",
        //    Name = "pizza",
        //    HasFieldType = false

        //});
    }

    #endregion

    #region SelectMenuItem Command

    public NinjaMvvm.Wpf.RelayCommand<MenuLineItem> SelectMenuItemCommand => new NinjaMvvm.Wpf.RelayCommand<MenuLineItem>((param) => _toastFaultlessExecutionService.TryExecute(() => this.SelectMenuItem(param)));

    public void SelectMenuItem(MenuLineItem item)
    {
        var vm = new ContentViewModel();
        if (item.IsAll)
        {
            //all
            foreach (var x in this.MenuLineItems)
            {
                if (!x.IsAll)
                {
                    vm.TableLineItems.Add(x.TableLineItem);
                }
            }
        }
        else
        {
            vm.TableLineItems.Add(item.TableLineItem);
        }

        this.MainContent = vm;
    }

    #endregion

    public ObservableCollection<MenuLineItem> MenuLineItems { get; } = new ObservableCollection<MenuLineItem>();

    public ContentViewModel MainContent
    {
        get { return GetField<ContentViewModel>(); }
        set { SetField(value); }
    }

    public class MenuLineItem : NinjaMvvm.NotificationBase
    {
        public MenuLineItem()
        {

        }

        public MenuLineItem(TableLineItem tableLineItem)
        {
            this.Name = tableLineItem.Name;
            this.TableLineItem = tableLineItem;
            this.HasConfig = true;
        }
        public bool IsAll
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public string Name
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public bool HasConfig
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public TableLineItem TableLineItem
        {
            get { return GetField<TableLineItem>(); }
            set { SetField(value); }
        }

    }

    public class ContentViewModel : NinjaMvvm.NotificationBase
    {
        public ObservableCollection<TableLineItem> TableLineItems { get; } = new ObservableCollection<TableLineItem>();
    }


    public class TableLineItem : NinjaMvvm.NotificationBase
    {
        public TableLineItem(Models.TableSettingModel tableSettingModel, Abstractions.IFieldTypeProcessorProvider fieldTypeProcessorProvider)
        {
            this.Name = tableSettingModel.TableName;
            foreach (var c in tableSettingModel.ColumnSettings)
            {
                this.ColumnLineItems.Add(new ColumnLineItem(c, fieldTypeProcessorProvider));
            }
        }

        public string Name
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }



        public ObservableCollection<ColumnLineItem> ColumnLineItems { get; } = new ObservableCollection<ColumnLineItem>();

    }
    public class ColumnLineItem : NinjaMvvm.NotificationBase
    {
        public ColumnLineItem()
        {

        }

        public ColumnLineItem(Models.ColumnSettingModel columnSettingModel, Abstractions.IFieldTypeProcessorProvider fieldTypeProcessorProvider)
        {
            this.Name = columnSettingModel.ColumnName;
            var processor = fieldTypeProcessorProvider.Get(columnSettingModel.FieldTypeProcessorTypeName);
            this.FieldType = processor?.Name ?? columnSettingModel.FieldTypeProcessorTypeName;
            this.HasFieldType = true;
        }

        public string Name
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public string FieldType
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public bool HasFieldType
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

    }
}
