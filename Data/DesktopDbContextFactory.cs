using Microsoft.EntityFrameworkCore;
using HHRReports.Desktop.Services;

namespace HHRReports.Desktop.Data
{
    public interface IDesktopDbContextFactory
    {
        ApplicationDbContext CreateDbContext();
    }

    public class DesktopDbContextFactory : IDesktopDbContextFactory
    {
        private readonly IDesktopAuthenticationService _authService;
        private readonly ILogger<DesktopDbContextFactory> _logger;

        public DesktopDbContextFactory(IDesktopAuthenticationService authService, ILogger<DesktopDbContextFactory> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public ApplicationDbContext CreateDbContext()
        {
            if (!_authService.IsAuthenticated)
            {
                throw new InvalidOperationException("User must be authenticated to create database context");
            }

            var connectionString = _authService.GetConnectionString();
            _logger.LogDebug("Creating DbContext for user: {Username}", _authService.CurrentSession?.Username);

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString, options =>
            {
                options.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
                options.CommandTimeout(1200); // 20 minutes
            });

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}