using TicketTracker.ViewModels;

namespace TicketTracker.Views;

public partial class OperatorPage : ContentPage
{
    private readonly OperatorViewModel _viewModel;

    public OperatorPage(OperatorViewModel viewModel)
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
