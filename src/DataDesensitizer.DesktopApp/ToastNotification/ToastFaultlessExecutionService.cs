using Microsoft.Extensions.Logging;
using System;

namespace DataDesensitizer.DesktopApp.ToastNotification;
public class ToastFaultlessExecutionService : FaultlessExecution.FaultlessExecutionService, Abstractions.IToastFaultlessExecutionService
{
    private readonly ToastNotification.Abstractions.IToastNotificationService _toastNotificationService;

    public ToastFaultlessExecutionService(ToastNotification.Abstractions.IToastNotificationService toastNotificationService,
        ILogger<ToastFaultlessExecutionService> logger)
        : base(logger)
    {
        _toastNotificationService = toastNotificationService;
    }
    protected override void OnException(Exception ex)
    {
        base.OnException(ex);


        _toastNotificationService.ShowToast(ToastNotification.ToastType.Error, $"Error occurred: {ex.Message}");

    }
}