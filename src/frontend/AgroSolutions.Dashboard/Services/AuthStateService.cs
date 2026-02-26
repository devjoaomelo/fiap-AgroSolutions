namespace AgroSolutions.Dashboard.Services;

public class AuthStateService
{
    private string? _token;
    private bool _isAuthenticated;

    public event Action? OnChange;

    public bool IsAuthenticated => _isAuthenticated;
    public string? Token => _token;

    public void SetAuthenticated(string token)
    {
        _token = token;
        _isAuthenticated = true;
        NotifyStateChanged();
    }

    public void Logout()
    {
        _token = null;
        _isAuthenticated = false;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}