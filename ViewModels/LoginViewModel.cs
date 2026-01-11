using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TicketTracker.Services;

namespace TicketTracker.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private string login = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    public LoginViewModel(IAuthService authService, NavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
        Title = "Авторизация";
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
        {
            SetError("Введите логин и пароль");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            var user = await _authService.LoginAsync(Login, Password);
            
            if (user != null)
            {
                await _navigationService.NavigateToRolePageAsync(user.Role);
                
                // Очистка полей после успешного входа
                Login = string.Empty;
                Password = string.Empty;
            }
            else
            {
                SetError("Неверный логин или пароль");
            }
        }
        catch (Exception ex)
        {
            SetError($"Ошибка входа: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ClearFields()
    {
        Login = string.Empty;
        Password = string.Empty;
        ClearError();
    }
}
