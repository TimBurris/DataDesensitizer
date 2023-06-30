using Notifications.Wpf.Core;

namespace DataDesensitizer.DesktopApp.ToastNotification;

/*
	DEVELOPER NOTE!!!!!:
	 to use this you need to add the following to your MainWindow
    xmlns:notifications="clr-namespace:Notifications.Wpf.Core.Controls;assembly=Notifications.Wpf.Core"


<notifications:NotificationArea  
     MaxItems="3"
     x:Name="WindowArea"
     Position="TopLeft" />
	 */
public class ToastNotificationService : Abstractions.IToastNotificationService
{
    private const string _notificationAreaName = "WindowArea";//this directly corresponds to what you name you notificationArea in your xaml
    private NotificationManager _notifier;
    public ToastNotificationService()
    {
        if (_notifier == null)
        {
            _notifier = new NotificationManager(System.Windows.Application.Current.Dispatcher);
        }
    }
    public void ShowToast(ToastType toastType, string message)
    {
        this.ShowToast(toastType, message, userAction: null);
    }

    public void ShowToast(ToastType toastType, string message, Action userAction)
    {
        var notificationContent = new NotificationContent
        {
            //Title = "Notification",
            Message = message,
            Type = ToNotificationType(toastType),
        };
        _notifier.ShowAsync(notificationContent, areaName: _notificationAreaName, onClick: userAction);
    }

    private NotificationType ToNotificationType(ToastType toastType)
    {
        switch (toastType)
        {
            case ToastType.Error:
                return NotificationType.Error;
            case ToastType.Success:
                return NotificationType.Success;
            case ToastType.Warning:
                return NotificationType.Warning;
            case ToastType.Info:
                return NotificationType.Information;

            default: throw new NotSupportedException(toastType.ToString());
        }
    }
}
