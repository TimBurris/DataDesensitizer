namespace DataDesensitizer.DesktopApp;

public class ConfirmationDialogModel
{
    public Action<bool>? OnComplete { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public string? AffirmativeButtonText { get; set; }
    public string? NegativeButtonText { get; set; }
}