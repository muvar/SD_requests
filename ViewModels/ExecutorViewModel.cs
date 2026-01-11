using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TicketTracker.Models;
using TicketTracker.Services;

namespace TicketTracker.ViewModels;

public partial class ExecutorViewModel : BaseViewModel
{
    private readonly ITicketService _ticketService;
    private readonly IAuthService _authService;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<Ticket> assignedTickets = new();

    [ObservableProperty]
    private Ticket? selectedTicket;

    [ObservableProperty]
    private ObservableCollection<TicketHistory> ticketHistory = new();

    [ObservableProperty]
    private string newComment = string.Empty;

    [ObservableProperty]
    private string resolution = string.Empty;

    [ObservableProperty]
    private TicketStatus selectedStatus = TicketStatus.InProgress;

    public static TicketStatus[] StatusOptions { get; } = { TicketStatus.InProgress, TicketStatus.Completed, TicketStatus.Postponed };

    public ExecutorViewModel(ITicketService ticketService, IAuthService authService, NavigationService navigationService)
    {
        _ticketService = ticketService;
        _authService = authService;
        _navigationService = navigationService;
        Title = "Исполнитель";
    }

    public async Task InitializeAsync()
    {
        await LoadAssignedTicketsAsync();
    }

    [RelayCommand]
    private async Task LoadAssignedTicketsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ClearError();

            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser != null)
            {
                var tickets = await _ticketService.GetAssignedTicketsAsync(currentUser.Id);
                AssignedTickets.Clear();
                foreach (var ticket in tickets)
                {
                    AssignedTickets.Add(ticket);
                }
            }
        }
        catch (Exception ex)
        {
            SetError($"Ошибка загрузки назначенных заявок: {ex.Message}");
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
            SelectedStatus = ticket.Status;
            Resolution = ticket.Resolution ?? string.Empty;
            
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
    private async Task UpdateStatusAsync()
    {
        if (SelectedTicket == null)
        {
            SetError("Выберите заявку для изменения статуса");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            await _ticketService.UpdateTicketStatusAsync(SelectedTicket.Id, SelectedStatus);
            await LoadAssignedTicketsAsync();
            
            // Обновить выбранную заявку
            var updatedTicket = await _ticketService.GetTicketByIdAsync(SelectedTicket.Id);
            if (updatedTicket != null)
            {
                await SelectTicketAsync(updatedTicket);
            }
        }
        catch (Exception ex)
        {
            SetError($"Ошибка изменения статуса: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CompleteTicketAsync()
    {
        if (SelectedTicket == null)
        {
            SetError("Выберите заявку для завершения");
            return;
        }

        if (string.IsNullOrWhiteSpace(Resolution))
        {
            SetError("Введите описание решения");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            await _ticketService.CloseTicketAsync(SelectedTicket.Id, Resolution);
            await LoadAssignedTicketsAsync();
            
            // Очистить выбранную заявку
            SelectedTicket = null;
            TicketHistory.Clear();
            Resolution = string.Empty;
        }
        catch (Exception ex)
        {
            SetError($"Ошибка завершения заявки: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task PostponeTicketAsync()
    {
        if (SelectedTicket == null)
        {
            SetError("Выберите заявку для отложения");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            await _ticketService.UpdateTicketStatusAsync(SelectedTicket.Id, TicketStatus.Postponed);
            await LoadAssignedTicketsAsync();
            
            // Обновить выбранную заявку
            var updatedTicket = await _ticketService.GetTicketByIdAsync(SelectedTicket.Id);
            if (updatedTicket != null)
            {
                await SelectTicketAsync(updatedTicket);
            }
        }
        catch (Exception ex)
        {
            SetError($"Ошибка отложения заявки: {ex.Message}");
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
        await LoadAssignedTicketsAsync();
    }
}
