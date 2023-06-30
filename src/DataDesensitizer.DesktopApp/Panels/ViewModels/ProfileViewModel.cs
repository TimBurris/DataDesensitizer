
using DataDesensitizer.Abstractions;
using DataDesensitizer.DesktopApp.DatabaseInspector.Abstractions;
using DataDesensitizer.DesktopApp.Flyouts.ViewModels;
using DataDesensitizer.DesktopApp.Navigator.Abstractions;
using DataDesensitizer.DesktopApp.ToastNotification.Abstractions;
using System.Threading;

namespace DataDesensitizer.DesktopApp.Panels.ViewModels;

public class ProfileViewModel : NinjaMvvm.Wpf.WpfViewModelBase
{
    private readonly IFieldTypeProcessorProvider _fieldTypeProcessorProvider;
    private readonly INavigator _navigator;
    private readonly ITableRepository _tableRepository;
    private readonly IColumnRepository _columnRepository;
    private readonly IProfileProcessor _profileProcessor;
    private readonly SelectDatabaseConnectionFlyoutViewModel _selectDatabaseConnectionFlyoutViewModel;
    private readonly IToastFaultlessExecutionService _toastFaultlessExecutionService;
    private readonly IToastNotificationService _toastNotificationService;
    private Models.ProfileModel? _profileModel;
    private List<FieldTypeLineItem> _fieldTypeLineItems = new List<FieldTypeLineItem>();

    public ProfileViewModel(Abstractions.IFieldTypeProcessorProvider fieldTypeProcessorProvider,
        Navigator.Abstractions.INavigator navigator,
        DatabaseInspector.Abstractions.ITableRepository tableRepository,
        DatabaseInspector.Abstractions.IColumnRepository columnRepository,
        Abstractions.IProfileProcessor profileProcessor,
        Flyouts.ViewModels.SelectDatabaseConnectionFlyoutViewModel selectDatabaseConnectionFlyoutViewModel,
        ToastNotification.Abstractions.IToastFaultlessExecutionService toastFaultlessExecutionService,
        ToastNotification.Abstractions.IToastNotificationService toastNotificationService)
    {
        _fieldTypeProcessorProvider = fieldTypeProcessorProvider;
        _navigator = navigator;
        _tableRepository = tableRepository;
        _columnRepository = columnRepository;
        _profileProcessor = profileProcessor;
        _selectDatabaseConnectionFlyoutViewModel = selectDatabaseConnectionFlyoutViewModel;
        _toastFaultlessExecutionService = toastFaultlessExecutionService;
        _toastNotificationService = toastNotificationService;
    }

    protected override async Task<bool> OnReloadDataAsync(CancellationToken cancellationToken)
    {

        //build field types before loading menu items
        if (!this.HasEverBeenLoaded)
        {
            this.BuildFieldTypeLineItems();
        }

        await this.LoadMenuItemsAsync();

        return await base.OnReloadDataAsync(cancellationToken);
    }

    private void BuildFieldTypeLineItems()
    {
        _fieldTypeLineItems.Clear();

        foreach (var p in _fieldTypeProcessorProvider.GetAll().OrderBy(x => x.Name))
        {
            _fieldTypeLineItems.Add(new FieldTypeLineItem(p.Name, p.GetType().AssemblyQualifiedName ?? "-unknown-"));
        }
    }

    protected override void OnUnloaded()
    {
        base.OnUnloaded();
        _selectDatabaseConnectionFlyoutViewModel.FlyoutConfirmed -= this._selectDatabaseConnectionFlyoutViewModel_FlyoutConfirmed;
    }

    protected override void OnBound()
    {
        base.OnBound();
        _selectDatabaseConnectionFlyoutViewModel.FlyoutConfirmed += this._selectDatabaseConnectionFlyoutViewModel_FlyoutConfirmed;
    }

    private void _selectDatabaseConnectionFlyoutViewModel_FlyoutConfirmed(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_selectDatabaseConnectionFlyoutViewModel.ConnectionString))
        {
            return;
        }

        this.ConnectionString = _selectDatabaseConnectionFlyoutViewModel.ConnectionString;
        var t = this.ReloadDataAsync();
    }

    public void AssignProfile(Models.ProfileModel profileModel)
    {
        _profileModel = profileModel;
    }

    private async Task LoadMenuItemsAsync()
    {
        this.MenuLineItems.Clear();

        //for now i'm not allowing this, it seems kinda useless
        //this.MenuLineItems.Add(new MenuLineItem()
        //{
        //    IsAll = true,
        //    Name = "All W/ Config"
        //});
        var existingTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (_profileModel != null)
        {
            foreach (var t in _profileModel.TableSettings)
            {
                var menuItem = new MenuLineItem(new TableLineItem(t, _fieldTypeLineItems));
                this.MenuLineItems.Add(menuItem);

                if (!existingTables.Contains(menuItem.Name))
                {
                    existingTables.Add(menuItem.Name);
                }
            }
        }

        var tableResult = await _toastFaultlessExecutionService.TryExecuteSyncAsAsync(() => _tableRepository.GetAll(this.ConnectionString));
        if (!tableResult.WasSuccessful)
            return;

        var allTables = tableResult.ReturnValue;
        foreach (var table in allTables.OrderBy(x => x.SchemaName).ThenBy(x => x.TableName))
        {
            var x = new Models.TableSettingModel(table.SchemaName, table.TableName);
            var menuItem = new MenuLineItem(new TableLineItem(x, _fieldTypeLineItems));

            if (!existingTables.Contains(menuItem.Name))
            {
                this.MenuLineItems.Add(menuItem);
            }
        }
    }

    private async Task LoadColumnsAsync(TableLineItem tableLineItem)
    {
        var existingColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var x in tableLineItem.ColumnLineItems)
        {
            if (!existingColumns.Contains(x.Name))
            {
                existingColumns.Add(x.Name);
            }
        }

        var columns = await Task.Run(() => _columnRepository.GetAllForTable(schemaName: tableLineItem.SchemaName, tableName: tableLineItem.TableName, connectionString: this.ConnectionString));
        foreach (var column in columns.OrderBy(x => x.ColumnName))
        {
            if (!existingColumns.Contains(column.ColumnName))
            {
                var item = new ColumnLineItem(column.ColumnName, _fieldTypeLineItems);
                tableLineItem.ColumnLineItems.Add(item);
            }
        }

        tableLineItem.HasBeenLoadedFromDatabase = true;
    }

    #region SelectMenuItem Command

    public NinjaMvvm.Wpf.RelayCommand<MenuLineItem> SelectMenuItemCommand => new NinjaMvvm.Wpf.RelayCommand<MenuLineItem>((param) => _toastFaultlessExecutionService.TryExecuteAsync(() => this.SelectMenuItemAsync(param)));

    public async Task SelectMenuItemAsync(MenuLineItem item)
    {
        var vm = new ContentViewModel();
        this.IsBusy = true;
        try
        {
            if (item.IsAll)
            {
                //all
                foreach (var x in this.MenuLineItems)
                {
                    if (!x.IsAll && x.HasConfig && x.TableLineItem != null)
                    {
                        vm.TableLineItems.Add(x.TableLineItem);
                    }
                }
            }
            else
            {
                if (item.TableLineItem != null)
                {
                    vm.TableLineItems.Add(item.TableLineItem);
                }
                else
                {
                    //really shouldn't see this....if it's not "all" there should be tablelineitem
                }
            }

            foreach (var t in vm.TableLineItems)
            {
                if (!t.HasBeenLoadedFromDatabase)
                {
                    await this.LoadColumnsAsync(t);
                }
            }
        }
        finally
        {
            this.IsBusy = false;
        }


        this.MainContent = vm;

    }

    #endregion

    #region ConfirmRun Command

    public NinjaMvvm.Wpf.RelayCommand ConfirmRunCommand => new NinjaMvvm.Wpf.RelayCommand((param) => _toastFaultlessExecutionService.TryExecuteAsync(() => this.ConfirmRunAsync()));

    public Task ConfirmRunAsync()
    {
        var model = new ConfirmationDialogModel()
        {
            Title = "Confirm",
            Message = "Are you sure you would like to release the Desensitization Ninja?",
            OnComplete = confirmed =>
            {
                if (confirmed)
                {
                    this.RunProcessorCommand.Execute();
                }
            }
        };
        return _navigator.ShowConfirmationDialogAsync(model);
    }

    #endregion

    #region RunProcessor Command

    public NinjaMvvm.Wpf.RelayCommand RunProcessorCommand => new NinjaMvvm.Wpf.RelayCommand((param) => _toastFaultlessExecutionService.TryExecuteAsync(() => this.RunProcessorAsync()));

    public async Task RunProcessorAsync()
    {
        this.IsBusy = true;

        try
        {
            var profile = this.BuildProfileFromSelections();
            if (!profile.TableSettings.Any())
            {
                _toastNotificationService.ShowToast(ToastNotification.ToastType.Warning, "Profile empty");
                return;
            }

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


    #region ChangeDatabaseConnection Command

    public NinjaMvvm.Wpf.RelayCommand ChangeDatabaseConnectionCommand => new NinjaMvvm.Wpf.RelayCommand((param) => _toastFaultlessExecutionService.TryExecute(() => this.ChangeDatabaseConnection()));

    public void ChangeDatabaseConnection()
    {
        _selectDatabaseConnectionFlyoutViewModel.Reset();
        _selectDatabaseConnectionFlyoutViewModel.AssignConnectionString(this.ConnectionString);
        _navigator.ShowFlyout(_selectDatabaseConnectionFlyoutViewModel);
    }

    #endregion

    private Models.ProfileModel BuildProfileFromSelections()
    {
        var p = new Models.ProfileModel(profileName: "TODO: name");

        foreach (var menuItem in this.MenuLineItems.Where(x => x.HasConfig))
        {
            var lineItem = menuItem.TableLineItem!;//remove null warning because "HasConfig" means there is a model
            var t = new Models.TableSettingModel(schemaName: lineItem.SchemaName, tableName: lineItem.TableName);
            t.Randomize = true;//one day this will be a setting, for now we default to true
            p.TableSettings.Add(t);

            foreach (var columnLineItem in lineItem.ColumnLineItems.Where(x => x.HasSelectedFieldType))
            {
                var selectedProcessor = columnLineItem.FieldTypes.First(x => x.IsSelected);//we can safely call first because HasSelectedFieldType is true
                var c = new Models.ColumnSettingModel(columnName: columnLineItem.Name, fieldTypeProcessorTypeName: selectedProcessor.FieldTypeProcessorTypeName);

                t.ColumnSettings.Add(c);
            }
        }

        return p;
    }

    public ObservableCollection<MenuLineItem> MenuLineItems { get; } = new ObservableCollection<MenuLineItem>();

    public ContentViewModel MainContent
    {
        get { return GetField<ContentViewModel>(); }
        set { SetField(value); }
    }

    public string ConnectionString
    {
        get { return GetField<string>(); }
        set { SetField(value); }
    }

    public class MenuLineItem : NinjaMvvm.NotificationBase
    {
        public MenuLineItem()
        {

        }

        public MenuLineItem(TableLineItem tableLineItem)
        {
            this.Name = tableLineItem.DisplayName;
            this.TableLineItem = tableLineItem;
            this.TableLineItem.ColumnLineItems.ItemPropertyChangedEvent += this.ColumnLineItems_ItemPropertyChangedEvent;
            this.TableLineItem.ColumnLineItems.CollectionChanged += this.ColumnLineItems_CollectionChanged;
            this.ReComputeHasConfig();
        }

        private void ColumnLineItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.ReComputeHasConfig();
        }

        private void ColumnLineItems_ItemPropertyChangedEvent(object sender, NinjaMvvm.ItemPropertyChangedEventArgs<ColumnLineItem> e)
        {
            if (this.TableLineItem == null)
                return;

            if (e.PropertyName == nameof(ColumnLineItem.HasSelectedFieldType))
            {
                this.ReComputeHasConfig();
            }
        }
        private void ReComputeHasConfig()
        {
            if (this.TableLineItem == null)
                return;

            this.HasConfig = this.TableLineItem.ColumnLineItems.Any(x => x.HasSelectedFieldType);
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

        public TableLineItem? TableLineItem
        {
            get { return GetField<TableLineItem?>(); }
            private set { SetField(value); }
        }

    }

    public class ContentViewModel : NinjaMvvm.NotificationBase
    {
        public ObservableCollection<TableLineItem> TableLineItems { get; } = new ObservableCollection<TableLineItem>();
    }


    public class TableLineItem : NinjaMvvm.NotificationBase
    {
        public TableLineItem(Models.TableSettingModel tableSettingModel, IEnumerable<FieldTypeLineItem> fieldTypeLineItems)
        {
            this.SchemaName = tableSettingModel.SchemaName;
            this.TableName = tableSettingModel.TableName;

            foreach (var c in tableSettingModel.ColumnSettings)
            {
                this.ColumnLineItems.Add(new ColumnLineItem(c, fieldTypeLineItems));
            }
        }

        public string DisplayName => $"{this.SchemaName}.{this.TableName}";

        public string SchemaName
        {
            get { return GetField<string>(); }
            set { SetField(value, additonalPropertiesToNotify: () => DisplayName); }
        }

        public string TableName
        {
            get { return GetField<string>(); }
            set { SetField(value, additonalPropertiesToNotify: () => DisplayName); }
        }

        public bool HasBeenLoadedFromDatabase
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public NinjaMvvm.NotificationObservableCollection<ColumnLineItem> ColumnLineItems { get; } = new();

    }
    public class ColumnLineItem : NinjaMvvm.NotificationBase
    {
        public ColumnLineItem(string name,
            IEnumerable<FieldTypeLineItem> fieldTypes)
        {
            this.Name = name;
            this.FieldTypes = this.BuildSelectableFieldTypes(fieldTypes, selectedFieldType: null);
            this.FieldTypes.ItemPropertyChangedEvent += this.FieldTypes_ItemPropertyChangedEvent;
            this.FieldTypes.CollectionChanged += this.FieldTypes_CollectionChanged;
            this.RecomputeHasSelectedFieldType();
        }

        public ColumnLineItem(Models.ColumnSettingModel columnSettingModel,
            IEnumerable<FieldTypeLineItem> fieldTypes)
        {
            this.Name = columnSettingModel.ColumnName;
            string selectedFieldType = fieldTypes.FirstOrDefault(x => string.Equals(x.FieldTypeProcessorTypeName, columnSettingModel.FieldTypeProcessorTypeName, StringComparison.OrdinalIgnoreCase))?.Name
                                ?? columnSettingModel.FieldTypeProcessorTypeName;
            this.FieldTypes = this.BuildSelectableFieldTypes(fieldTypes, selectedFieldType);
            this.FieldTypes.ItemPropertyChangedEvent += this.FieldTypes_ItemPropertyChangedEvent;
            this.FieldTypes.CollectionChanged += this.FieldTypes_CollectionChanged;
            this.RecomputeHasSelectedFieldType();
        }

        private void FieldTypes_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.RecomputeHasSelectedFieldType();
        }

        private void FieldTypes_ItemPropertyChangedEvent(object sender, NinjaMvvm.ItemPropertyChangedEventArgs<SelectableFieldTypeLineItem> e)
        {
            if (e.PropertyName == nameof(SelectableFieldTypeLineItem.IsSelected))
            {
                this.RecomputeHasSelectedFieldType();
            }
        }

        private void RecomputeHasSelectedFieldType()
        {
            this.HasSelectedFieldType = this.FieldTypes.Any(x => x.IsSelected);
        }

        private NinjaMvvm.NotificationObservableCollection<SelectableFieldTypeLineItem> BuildSelectableFieldTypes(IEnumerable<FieldTypeLineItem> fieldTypes, string? selectedFieldType)
        {
            var items = fieldTypes.Select(x => new SelectableFieldTypeLineItem(x)
            {
                IsSelected = string.Equals(selectedFieldType, x.Name, StringComparison.OrdinalIgnoreCase),
            }).ToList();

            return new NinjaMvvm.NotificationObservableCollection<SelectableFieldTypeLineItem>(items);
        }

        public bool HasSelectedFieldType
        {
            get { return GetField<bool>(); }
            private set { SetField(value); }
        }

        public string Name
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }


        #region SelectFieldType Command

        public NinjaMvvm.Wpf.RelayCommand<SelectableFieldTypeLineItem> SelectFieldTypeCommand => new NinjaMvvm.Wpf.RelayCommand<SelectableFieldTypeLineItem>((param) => this.SelectFieldType(param));

        public void SelectFieldType(SelectableFieldTypeLineItem item)
        {
            foreach (var t in this.FieldTypes)
            {
                //if they chose the selected one, then toggle it off
                if (t == item && t.IsSelected)
                {
                    t.IsSelected = false;
                }
                else
                {
                    t.IsSelected = item == t;
                }
            }
        }

        #endregion

        public NinjaMvvm.NotificationObservableCollection<SelectableFieldTypeLineItem> FieldTypes { get; }
        public class SelectableFieldTypeLineItem : NinjaMvvm.NotificationBase
        {
            public SelectableFieldTypeLineItem(FieldTypeLineItem fieldTypeLineItem)
            {
                this.Name = fieldTypeLineItem.Name;
                this.FieldTypeProcessorTypeName = fieldTypeLineItem.FieldTypeProcessorTypeName;
            }
            public bool IsSelected
            {
                get { return GetField<bool>(); }
                set { SetField(value); }
            }

            public string Name { get; }
            public string FieldTypeProcessorTypeName { get; }
        }
    }

    public class FieldTypeLineItem
    {
        public FieldTypeLineItem(string name, string fieldTypeProcessorTypeName)
        {
            this.Name = name;
            this.FieldTypeProcessorTypeName = fieldTypeProcessorTypeName;
        }
        public string Name { get; }
        public string FieldTypeProcessorTypeName { get; }
    }
}
