namespace AgroSolutions.Dashboard.Services;

public class AuthStateService
{
    private string? _token;
    private Guid? _userId;
    private string? _userName;
    private string? _userEmail;
    private bool _isAuthenticated;

    public event Action? OnChange;

    public bool IsAuthenticated => _isAuthenticated;
    public string? Token => _token;
    public Guid? UserId => _userId;
    public string? UserName => _userName;
    public string? UserEmail => _userEmail;

    public void SetAuthenticated(string token, Guid userId, string userName, string userEmail)
    {
        _token = token;
        _userId = userId;
        _userName = userName;
        _userEmail = userEmail;
        _isAuthenticated = true;
        NotifyStateChanged();
    }

    public void Logout()
    {
        _token = null;
        _userId = null;
        _userName = null;
        _userEmail = null;
        _isAuthenticated = false;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}