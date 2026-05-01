using Blazored.LocalStorage;
using DTOs;
using IServices;

namespace Web.Services;

public class SessionService
{
    private readonly IAuthService _authService;
    private readonly ILocalStorageService _localStorage;
    private LoginResponse? _currentUser;

    public bool IsInitialized { get; private set; } = false;

    public SessionService(IAuthService authService, ILocalStorageService localStorage)
    {
        _authService = authService;
        _localStorage = localStorage;
    }

    public async Task InitializeAsync()
    {
        var stored = await _localStorage.GetItemAsync<LoginResponse>("loggedUser");
        _currentUser = stored;
        IsInitialized = true;
    }

    public async Task Login(LoginRequest request)
    {
        _currentUser = _authService.Login(request);
        await _localStorage.SetItemAsync("loggedUser", _currentUser);
        IsInitialized = true;
    }

    public bool IsUserActive()
    {
        return _currentUser is not null;
    }

    public async Task LogOut()
    {
        _currentUser = null;
        await _localStorage.RemoveItemAsync("loggedUser");
    }

    public LoginResponse GetCurrentUser()
    {
        return _currentUser!;
    }
}