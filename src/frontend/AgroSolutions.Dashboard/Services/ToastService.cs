namespace AgroSolutions.Dashboard.Services;

public class ToastService
{
    public event Action<string, ToastType>? OnShow;

    public void ShowSuccess(string message)
    {
        Console.WriteLine($"[TOAST] ShowSuccess called: {message}");
        OnShow?.Invoke(message, ToastType.Success);
    }

    public void ShowError(string message)
    {
        OnShow?.Invoke(message, ToastType.Error);
    }

    public void ShowInfo(string message)
    {
        OnShow?.Invoke(message, ToastType.Info);
    }
}

public enum ToastType
{
    Success,
    Error,
    Info
}