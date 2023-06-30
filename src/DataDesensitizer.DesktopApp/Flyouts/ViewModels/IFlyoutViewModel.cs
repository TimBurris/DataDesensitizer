namespace DataDesensitizer.DesktopApp.Flyouts.ViewModels;

public interface IFlyoutViewModel
{

    /// <summary>
    /// if the flyout needs to broadcast "confirmed" (as opposed to simple close/cancel) they'd do that using this event
    /// </summary>
    event EventHandler? FlyoutConfirmed;

    /// <summary>
    /// the flyout's host will assign this;  the flyout can then use it when it's ready to close/hide 
    /// </summary>
    System.Windows.Input.ICommand? HideFlyoutCommand { get; set; }

    /// <summary>
    /// the title of the flyout
    /// </summary>
    string? ViewTitle { get; set; }
}
