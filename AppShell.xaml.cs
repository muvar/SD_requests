using TicketTracker.Views;

namespace TicketTracker;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Регистрация маршрутов
        Routing.RegisterRoute("operator", typeof(OperatorPage));
        Routing.RegisterRoute("specialist", typeof(SpecialistPage));
        Routing.RegisterRoute("executor", typeof(ExecutorPage));
        Routing.RegisterRoute("admin", typeof(AdminPage));
    }
}
