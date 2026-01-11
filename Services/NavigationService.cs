namespace TicketTracker.Services;

public class NavigationService
{
    public async Task NavigateToAsync(string route)
    {
        await Shell.Current.GoToAsync(route);
    }

    public async Task NavigateToAsync(string route, IDictionary<string, object> parameters)
    {
        await Shell.Current.GoToAsync(route, parameters);
    }

    public async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    public async Task NavigateToRolePageAsync(Models.UserRole role)
    {
        string route = role switch
        {
            Models.UserRole.Operator => "operator",
            Models.UserRole.Specialist => "specialist",
            Models.UserRole.Executor => "executor",
            Models.UserRole.Administrator => "admin",
            _ => "LoginPage"
        };

        await NavigateToAsync($"//{route}");
    }
}
