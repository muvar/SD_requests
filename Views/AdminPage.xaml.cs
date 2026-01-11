using TicketTracker.ViewModels;

namespace TicketTracker.Views;

public partial class AdminPage : ContentPage
{
    private readonly AdminViewModel _viewModel;

    public AdminPage(AdminViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}
