using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TicketTracker.Data;
using TicketTracker.Services;
using TicketTracker.ViewModels;
using TicketTracker.Views;

namespace TicketTracker;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Регистрация DbContext
        builder.Services.AddDbContext<TicketTrackerContext>(options =>
            options.UseSqlServer("Server=localhost;Database=TicketTrackerDB;Trusted_Connection=true;TrustServerCertificate=true;"));

        // Регистрация сервисов
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<ITicketService, TicketService>();
        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddSingleton<NavigationService>();

        // Регистрация ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<OperatorViewModel>();
        builder.Services.AddTransient<SpecialistViewModel>();
        builder.Services.AddTransient<ExecutorViewModel>();
        builder.Services.AddTransient<AdminViewModel>();

        // Регистрация Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<OperatorPage>();
        builder.Services.AddTransient<SpecialistPage>();
        builder.Services.AddTransient<ExecutorPage>();
        builder.Services.AddTransient<AdminPage>();

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddLogging(configure => configure.AddDebug());
#endif

        return builder.Build();
    }
}
