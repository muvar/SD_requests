using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TicketTracker.Models;
using TicketTracker.Services;

namespace TicketTracker.ViewModels;

public partial class AdminViewModel : BaseViewModel
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<User> users = new();

    [ObservableProperty]
    private User? selectedUser;

    [ObservableProperty]
    private string newUserLogin = string.Empty;

    [ObservableProperty]
    private string newUserPassword = string.Empty;

    [ObservableProperty]
    private string newUserFullName = string.Empty;

    [ObservableProperty]
    private UserRole newUserRole = UserRole.Operator;

    [ObservableProperty]
    private bool isCreateUserVisible;

    public static UserRole[] RoleOptions { get; } = Enum.GetValues<UserRole>();

    public AdminViewModel(IUserService userService, IAuthService authService, NavigationService navigationService)
    {
        _userService = userService;
        _authService = authService;
        _navigationService = navigationService;
        Title = "Администратор";
    }

    public async Task InitializeAsync()
    {
        await LoadUsersAsync();
    }

    [RelayCommand]
    private async Task LoadUsersAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ClearError();

            var allUsers = await _userService.GetAllUsersAsync();
            Users.Clear();
            foreach (var user in allUsers)
            {
                Users.Add(user);
            }
        }
        catch (Exception ex)
        {
            SetError($"Ошибка загрузки пользователей: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void SelectUser(User user)
    {
        SelectedUser = user;
    }

    [RelayCommand]
    private void ShowCreateUser()
    {
        IsCreateUserVisible = true;
        ClearCreateUserFields();
    }

    [RelayCommand]
    private void HideCreateUser()
    {
        IsCreateUserVisible = false;
        ClearCreateUserFields();
    }

    [RelayCommand]
    private async Task CreateUserAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(NewUserLogin) || 
            string.IsNullOrWhiteSpace(NewUserPassword) || 
            string.IsNullOrWhiteSpace(NewUserFullName))
        {
            SetError("Заполните все поля для создания пользователя");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            // Проверить, не существует ли уже пользователь с таким логином
            var existingUser = await _userService.GetUserByLoginAsync(NewUserLogin);
            if (existingUser != null)
            {
                SetError("Пользователь с таким логином уже существует");
                return;
            }

            var newUser = new User
            {
                Login = NewUserLogin,
                FullName = NewUserFullName,
                Role = NewUserRole,
                IsActive = true
            };

            await _userService.CreateUserAsync(newUser, NewUserPassword);
            await LoadUsersAsync();
            HideCreateUser();
        }
        catch (Exception ex)
        {
            SetError($"Ошибка создания пользователя: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DeactivateUserAsync(User user)
    {
        if (user == null) return;

        try
        {
            IsBusy = true;
            ClearError();

            await _userService.DeactivateUserAsync(user.Id);
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            SetError($"Ошибка деактивации пользователя: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ActivateUserAsync(User user)
    {
        if (user == null) return;

        try
        {
            IsBusy = true;
            ClearError();

            await _userService.ActivateUserAsync(user.Id);
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            SetError($"Ошибка активации пользователя: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ResetPasswordAsync(User user)
    {
        if (user == null) return;

        try
        {
            IsBusy = true;
            ClearError();

            // Генерация нового пароля
            var newPassword = GenerateRandomPassword();
            await _userService.ChangePasswordAsync(user.Id, newPassword);
            
            // В реальном приложении здесь должно быть уведомление пользователя о новом пароле
            SetError($"Новый пароль для {user.FullName}: {newPassword}");
        }
        catch (Exception ex)
        {
            SetError($"Ошибка сброса пароля: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        await _navigationService.NavigateToAsync("//LoginPage");
    }

    private void ClearCreateUserFields()
    {
        NewUserLogin = string.Empty;
        NewUserPassword = string.Empty;
        NewUserFullName = string.Empty;
        NewUserRole = UserRole.Operator;
        ClearError();
    }

    private string GenerateRandomPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    protected override async Task RefreshAsync()
    {
        await LoadUsersAsync();
    }
}
