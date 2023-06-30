using System;

namespace DataDesensitizer.DesktopApp.ToastNotification.Abstractions;

public interface IToastNotificationService
{
	void ShowToast(ToastType toastType, string message);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="toastType"></param>
	/// <param name="message"></param>
	/// <param name="userAction">action that should executed if the user clicks or presses enter for the message</param>
	void ShowToast(ToastType toastType, string message, Action userAction);
}
