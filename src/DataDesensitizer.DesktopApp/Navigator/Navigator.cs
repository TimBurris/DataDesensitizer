using Microsoft.Extensions.DependencyInjection;

namespace DataDesensitizer.DesktopApp.Navigator;

public class Navigator : Abstractions.INavigator
{
    private readonly IServiceProvider _serviceProvider;

    public Navigator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public void ShowFlyout(Flyouts.ViewModels.IFlyoutViewModel viewModel)
    {
        //you can't constructor inject MainViewModel because it can create a circular dependency because MainViewModel has child view models which use navigator
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.ShowFlyout(viewModel);
    }
}
