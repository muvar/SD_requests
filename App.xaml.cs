using TicketTracker.Views;

namespace TicketTracker;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}
