using TicketTracker.ViewModels;

namespace TicketTracker.Views;

public partial class ExecutorPage : ContentPage
{
    private readonly ExecutorViewModel _viewModel;

    public ExecutorPage(ExecutorViewModel viewModel)
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
