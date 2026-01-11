using TicketTracker.ViewModels;

namespace TicketTracker.Views;

public partial class SpecialistPage : ContentPage
{
    private readonly SpecialistViewModel _viewModel;

    public SpecialistPage(SpecialistViewModel viewModel)
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
