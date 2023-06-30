using DataDesensitizer.DesktopApp.ToastNotification.Abstractions;
using FluentValidation;
using System.Windows.Input;

namespace DataDesensitizer.DesktopApp.Flyouts.ViewModels;
public class SelectDatabaseConnectionFlyoutViewModel : NinjaMvvm.Wpf.WpfViewModelBase, IFlyoutViewModel
{
    private readonly IToastFaultlessExecutionService _toastFaultlessExecutionService;

    public event EventHandler? FlyoutConfirmed;
    public ICommand? HideFlyoutCommand { get; set; }
    public override string ViewTitle => "Database Connection";
    public SelectDatabaseConnectionFlyoutViewModel(ToastNotification.Abstractions.IToastFaultlessExecutionService toastFaultlessExecutionService)
    {
        _toastFaultlessExecutionService = toastFaultlessExecutionService;

        this.SetDefaultValues();
    }

    private void SetDefaultValues()
    {
        this.ServerName = "(local)";
        this.DatabaseName = null;
        this.Username = null;
        this.Password = null;
        this.UseTrustedConnection = true;
        this.EncryptConnection = false;
        this.MultipleActiveResultSets = true;
    }
    public void Reset()
    {
        //defaults
        this.SetDefaultValues();
    }

    public void AssignConnectionString(string connectionString)
    {
        var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString);

        this.ServerName = builder.DataSource;
        this.DatabaseName = builder.InitialCatalog;
        this.UseTrustedConnection = builder.IntegratedSecurity;
        this.MultipleActiveResultSets = builder.MultipleActiveResultSets;
        this.EncryptConnection = builder.Encrypt;
        this.Username = builder.UserID;
        this.Password = builder.Password;
    }

    #region Binding Props

    public string? ConnectionString
    {
        get { return GetField<string?>(); }
        private set { SetField(value); }
    }

    public string? ServerName
    {
        get { return GetField<string?>(); }
        set { SetField(value); }
    }

    public string? DatabaseName
    {
        get { return GetField<string?>(); }
        set { SetField(value); }
    }

    public bool UseTrustedConnection
    {
        get { return GetField<bool>(); }
        set { SetField(value); }
    }

    public string? Username
    {
        get { return GetField<string?>(); }
        set { SetField(value); }
    }

    public string? Password
    {
        get { return GetField<string?>(); }
        set { SetField(value); }
    }

    public bool EncryptConnection
    {
        get { return GetField<bool>(); }
        set { SetField(value); }
    }

    public bool MultipleActiveResultSets
    {
        get { return GetField<bool>(); }
        set { SetField(value); }
    }

    #endregion

    #region Connect Command

    public NinjaMvvm.Wpf.RelayCommand ConnectCommand => new NinjaMvvm.Wpf.RelayCommand((param) => _toastFaultlessExecutionService.TryExecute(() => this.Connect()));

    public void Connect()
    {
        this.ShowErrors = false;
        if (!this.GetValidationResult().IsValid)
        {
            this.ShowErrors = true;
            return;
        }

        //test connection

        var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder
        {
            DataSource = this.ServerName,
            InitialCatalog = this.DatabaseName,
            IntegratedSecurity = this.UseTrustedConnection,
            MultipleActiveResultSets = this.MultipleActiveResultSets,
            Encrypt = this.EncryptConnection,
        };

        if (!this.UseTrustedConnection)
        {
            builder.UserID = this.Username;
            builder.Password = this.Password;
        }

        this.ConnectionString = builder.ConnectionString;

        this.FlyoutConfirmed?.Invoke(this, EventArgs.Empty);

        this.HideFlyoutCommand?.Execute(parameter: null);
    }

    #endregion

    #region SelectDatabaseConnectionFlyoutViewModel Validation

    class SelectDatabaseConnectionFlyoutViewModelValidator : AbstractValidator<SelectDatabaseConnectionFlyoutViewModel>
    {
        public SelectDatabaseConnectionFlyoutViewModelValidator()
        {

            RuleFor(obj => obj.ServerName).NotEmpty();
            RuleFor(obj => obj.DatabaseName).NotEmpty();
            RuleFor(obj => obj.Username).NotEmpty().When(x => !x.UseTrustedConnection);
            RuleFor(obj => obj.Password).NotEmpty().When(x => !x.UseTrustedConnection);
        }
    }

    protected override IValidator GetValidator()
    {
        return new SelectDatabaseConnectionFlyoutViewModelValidator();
    }

    #endregion

}
