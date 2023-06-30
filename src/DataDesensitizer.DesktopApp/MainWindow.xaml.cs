using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Input;

namespace DataDesensitizer.DesktopApp;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : MetroWindow
{
    private readonly MainViewModel _viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();

        this.DataContext = viewModel;
        _viewModel = viewModel;

        MahApps.Metro.Controls.Dialogs.DialogParticipation.SetRegister(this, _viewModel);//tell MahApps our ViewModel can show dialogs with this Window

        this.Loaded += this.MainWindow_Loaded;
        this.Unloaded += this.MainWindow_Unloaded;

        //#if DEBUG
        //        if (System.Windows.SystemParameters.PrimaryScreenWidth > 1920)
        //        {
        //            this.Width = 1920;
        //            this.Height = 1080;
        //            this.WindowState = WindowState.Normal;
        //            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        //        }
        //#else

        //            this.WindowState = WindowState.Maximized;
        //#endif


    }

    private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
    {
        _viewModel.ViewHasBeenUnbound();
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _viewModel.ViewHasBeenBound();
    }

    private void ToolBarButton_Click(object sender, RoutedEventArgs e)
    {
        //toolbar buttons cannot claim focus (because they are not in the normal ui hierarchy, so when we click a toolbar button force lost focus so that any ui controls that might be waiting on lost focus for binding will work
        this.ClearFocus();
    }
    void ClearFocus()
    {
        UIElement? elementWithFocus = Keyboard.FocusedElement as UIElement;
        if (elementWithFocus is System.Windows.Controls.TextBox tb)
        {
            if (Keyboard.FocusedElement != null)
            {
                Keyboard.FocusedElement.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));
                Keyboard.ClearFocus();
            }
        }
    }
}

