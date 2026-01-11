using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TicketTracker.Models;
using TicketTracker.Services;

namespace TicketTracker.ViewModels;

public partial class OperatorViewModel : BaseViewModel
{
    private readonly ITicketService _ticketService;
    private readonly IAuthService _authService;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<Ticket> tickets = new();

    [ObservableProperty]
    private Ticket? selectedTicket;

    [ObservableProperty]
    private ObservableCollection<TicketHistory> ticketHistory = new();

    [ObservableProperty]
    private string newTicketTitle = string.Empty;

    [ObservableProperty]
    private string newTicketDescription = string.Empty;

    [ObservableProperty]
    private TicketPriority newTicketPriority = TicketPriority.Medium;

    [ObservableProperty]
    private bool isCreateTicketVisible;

    public static TicketPriority[] PriorityOptions { get; } = Enum.GetValues<TicketPriority>();

    public OperatorViewModel(ITicketService ticketService, IAuthService authService, NavigationService navigationService)
    {
        _ticketService = ticketService;
        _authService = authService;
        _navigationService = navigationService;
        Title = "Оператор";
    }

    public async Task InitializeAsync()
    {
        await LoadTicketsAsync();
    }

    [RelayCommand]
    private async Task LoadTicketsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ClearError();

            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser != null)
            {
                var userTickets = await _ticketService.GetTicketsByUserAsync(currentUser.Id);
                Tickets.Clear();
                foreach (var ticket in userTickets)
                {
                    Tickets.Add(ticket);
                }
            }
        }
        catch (Exception ex)
        {
            SetError($"Ошибка загрузки заявок: {ex.Message}");
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
        }
        catch (Exception ex)
        {
            SetError($"Ошибка загрузки истории заявки: {ex.Message}");
        }
    }

    [RelayCommand]
    private void ShowCreateTicket()
    {
        IsCreateTicketVisible = true;
        NewTicketTitle = string.Empty;
        NewTicketDescription = string.Empty;
        NewTicketPriority = TicketPriority.Medium;
    }

    [RelayCommand]
    private void HideCreateTicket()
    {
        IsCreateTicketVisible = false;
    }

    [RelayCommand]
    private async Task CreateTicketAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(NewTicketTitle) || string.IsNullOrWhiteSpace(NewTicketDescription))
        {
            SetError("Заполните все поля для создания заявки");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser != null)
            {
                var newTicket = new Ticket
                {
                    Title = NewTicketTitle,
                    Description = NewTicketDescription,
                    Priority = NewTicketPriority,
                    CreatedById = currentUser.Id,
                    Status = TicketStatus.New
                };

                await _ticketService.CreateTicketAsync(newTicket);
                await LoadTicketsAsync();
                HideCreateTicket();
            }
        }
        catch (Exception ex)
        {
            SetError($"Ошибка создания заявки: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task EditTicketAsync(Ticket ticket)
    {
        if (ticket == null) return;

        // Здесь можно добавить логику редактирования заявки
        // Например, открыть модальное окно или перейти на страницу редактирования
    }

    [RelayCommand]
    private async Task CloseTicketAsync(Ticket ticket)
    {
        if (ticket == null) return;

        try
        {
            IsBusy = true;
            ClearError();

            await _ticketService.CloseTicketAsync(ticket.Id, "Закрыто оператором");
            await LoadTicketsAsync();
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
        await LoadTicketsAsync();
    }
}
