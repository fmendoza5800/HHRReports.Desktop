using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using HHRReports.Desktop.Data;
using HHRReports.Desktop.Services;
using System.Reflection;

namespace HHRReports.Desktop;

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
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Add configuration
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("HHRReports.Desktop.appsettings.json") 
            ?? File.OpenRead("appsettings.json");
        
        var configuration = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();
        
        builder.Services.AddSingleton<IConfiguration>(configuration);

        // Add logging
        builder.Logging.SetMinimumLevel(LogLevel.Information);

        // Register authentication service as singleton for desktop
        builder.Services.AddSingleton<DesktopAuthenticationService>();
        builder.Services.AddSingleton<IDesktopAuthenticationService>(provider => 
            provider.GetRequiredService<DesktopAuthenticationService>());
        builder.Services.AddSingleton<IAuthenticationService>(provider => 
            provider.GetRequiredService<DesktopAuthenticationService>());
        
        // Register DbContext factory
        builder.Services.AddSingleton<IDesktopDbContextFactory, DesktopDbContextFactory>();
        
        // Register services
        builder.Services.AddScoped<IPoolDetailService, PoolDetailService>();
        builder.Services.AddScoped<ITerminalDetailService, TerminalDetailService>();
        builder.Services.AddScoped<IPerformanceReportService, PerformanceReportService>();

        // Add required ASP.NET Core services
        builder.Services.AddAuthorizationCore();

        return builder.Build();
    }
}