using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TicketTracker.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    protected void SetError(string message)
    {
        ErrorMessage = message;
        HasError = !string.IsNullOrEmpty(message);
    }

    protected void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }

    [RelayCommand]
    protected virtual async Task RefreshAsync()
    {
        // Переопределяется в дочерних классах
        await Task.CompletedTask;
    }
}
