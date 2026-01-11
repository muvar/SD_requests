using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TicketTracker.Models;
using TicketTracker.Services;

namespace TicketTracker.ViewModels;

public partial class SpecialistViewModel : BaseViewModel
{
    private readonly ITicketService _ticketService;
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<Ticket> openTickets = new();

    [ObservableProperty]
    private ObservableCollection<Ticket> allTickets = new();

    [ObservableProperty]
    private ObservableCollection<User> executors = new();

    [ObservableProperty]
    private Ticket? selectedTicket;

    [ObservableProperty]
    private ObservableCollection<TicketHistory> ticketHistory = new();

    [ObservableProperty]
    private string newComment = string.Empty;

    [ObservableProperty]
    private User? selectedExecutor;

    public SpecialistViewModel(ITicketService ticketService, IUserService userService, IAuthService authService, NavigationService navigationService)
    {
        _ticketService = ticketService;
        _userService = userService;
        _authService = authService;
        _navigationService = navigationService;
        Title = "Специалист";
    }

    public async Task InitializeAsync()
    {
        await LoadDataAsync();
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ClearError();

            // Загрузка открытых заявок
            var openTicketsData = await _ticketService.GetOpenTicketsAsync();
            OpenTickets.Clear();
            foreach (var ticket in openTicketsData)
            {
                OpenTickets.Add(ticket);
            }

            // Загрузка всех заявок
            var allTicketsData = await _ticketService.GetAllTicketsAsync();
            AllTickets.Clear();
            foreach (var ticket in allTicketsData)
            {
                AllTickets.Add(ticket);
            }

            // Загрузка исполнителей
            var executorsData = await _userService.GetUsersByRoleAsync(UserRole.Executor);
            Executors.Clear();
            foreach (var executor in executorsData)
            {
                Executors.Add(executor);
            }
        }
        catch (Exception ex)
        {
            SetError($"Ошибка загрузки данных: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SelectTicketAsync(Ticket ticket)
    {
        if (ticket == null) return;

        try
        {
            SelectedTicket = ticket;
            var history = await _ticketService.GetTicketHistoryAsync(ticket.Id);
            TicketHistory.Clear();
            foreach (var entry in history)
            {
                TicketHistory.Add(entry);
            }

            // Установить текущего исполнителя, если есть
            SelectedExecutor = ticket.AssignedTo;
        }
        catch (Exception ex)
        {
            SetError($"Ошибка загрузки истории заявки: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task AddCommentAsync()
    {
        if (SelectedTicket == null || string.IsNullOrWhiteSpace(NewComment))
        {
            SetError("Выберите заявку и введите комментарий");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser != null)
            {
                var comment = new Comment
                {
                    TicketId = SelectedTicket.Id,
                    UserId = currentUser.Id,
                    Text = NewComment,
                    IsInternal = false
                };

                await _ticketService.AddCommentAsync(comment);
                NewComment = string.Empty;
                
                // Обновить историю заявки
                await SelectTicketAsync(SelectedTicket);
            }
        }
        catch (Exception ex)
        {
            SetError($"Ошибка добавления комментария: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AssignExecutorAsync()
    {
        if (SelectedTicket == null || SelectedExecutor == null)
        {
            SetError("Выберите заявку и исполнителя");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            await _ticketService.AssignTicketAsync(SelectedTicket.Id, SelectedExecutor.Id);
            await LoadDataAsync();
            
            // Обновить выбранную заявку
            var updatedTicket = await _ticketService.GetTicketByIdAsync(SelectedTicket.Id);
            if (updatedTicket != null)
            {
                await SelectTicketAsync(updatedTicket);
            }
        }
        catch (Exception ex)
        {
            SetError($"Ошибка назначения исполнителя: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CloseTicketAsync()
    {
        if (SelectedTicket == null)
        {
            SetError("Выберите заявку для закрытия");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            await _ticketService.CloseTicketAsync(SelectedTicket.Id, "Закрыто специалистом");
            await LoadDataAsync();
            
            // Очистить выбранную заявку
            SelectedTicket = null;
            TicketHistory.Clear();
        }
        catch (Exception ex)
        {
            SetError($"Ошибка закрытия заявки: {ex.Message}");
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

    protected override async Task RefreshAsync()
    {
        await LoadDataAsync();
    }
}
