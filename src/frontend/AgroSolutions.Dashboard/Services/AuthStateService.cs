using Microsoft.JSInterop;
using System.Text.Json;

namespace AgroSolutions.Dashboard.Services;

public class AuthStateService
{
    private readonly IJSRuntime _jsRuntime;
    private string? _token;
    private Guid? _userId;
    private string? _userName;
    private string? _userEmail;
    private bool _isAuthenticated;
    private bool _isInitialized = false;

    public event Action? OnChange;

    public bool IsAuthenticated => _isAuthenticated;
    public string? Token => _token;
    public Guid? UserId => _userId;
    public string? UserName => _userName;
    public string? UserEmail => _userEmail;
    public bool IsInitialized => _isInitialized;

    public AuthStateService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        try
        {
            var authDataJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authData");

            if (!string.IsNullOrEmpty(authDataJson))
            {
                var authData = JsonSerializer.Deserialize<AuthData>(authDataJson);
                if (authData != null)
                {
                    _token = authData.Token;
                    _userId = authData.UserId;
                    _userName = authData.UserName;
                    _userEmail = authData.UserEmail;
                    _isAuthenticated = true;
                }
            }
        }
        catch
        {
            // Se falhar ao carregar, ignora
        }
        finally
        {
            _isInitialized = true;
        }
    }

    public async Task SetAuthenticatedAsync(string token, Guid userId, string userName, string userEmail)
    {
        _token = token;
        _userId = userId;
        _userName = userName;
        _userEmail = userEmail;
        _isAuthenticated = true;

        var authData = new AuthData
        {
            Token = token,
            UserId = userId,
            UserName = userName,
            UserEmail = userEmail
        };

        var authDataJson = JsonSerializer.Serialize(authData);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authData", authDataJson);

        NotifyStateChanged();
    }

    public async Task LogoutAsync()
    {
        _token = null;
        _userId = null;
        _userName = null;
        _userEmail = null;
        _isAuthenticated = false;

        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authData");

        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    private class AuthData
    {
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
    }
}