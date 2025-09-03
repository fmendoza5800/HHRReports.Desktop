using Microsoft.EntityFrameworkCore;
using HHRReports.Desktop.Data;
using HHRReports.Desktop.Models;
using Microsoft.Data.SqlClient;

namespace HHRReports.Desktop.Services
{
    public interface IPoolDetailService
    {
        Task<List<PoolDetail>> GetPoolDetailsAsync(DateTime startDate, CancellationToken cancellationToken = default);
        Task<List<PoolDetail>> GetPoolDetailsAsync(DateTime startDate, string? authToken, CancellationToken cancellationToken = default);
    }

    public class PoolDetailService : IPoolDetailService
    {
        private readonly IDesktopDbContextFactory _contextFactory;
        private readonly ILogger<PoolDetailService> _logger;

        public PoolDetailService(
            IDesktopDbContextFactory contextFactory, 
            ILogger<PoolDetailService> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<List<PoolDetail>> GetPoolDetailsAsync(DateTime startDate, CancellationToken cancellationToken = default)
        {
            // Call the overloaded method with null authToken
            return await GetPoolDetailsAsync(startDate, null, cancellationToken);
        }

        public async Task<List<PoolDetail>> GetPoolDetailsAsync(DateTime startDate, string? authToken, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Starting GetPoolDetailsAsync for start date: {StartDate}", startDate);

                using var context = _contextFactory.CreateDbContext();
                
                // Test database connection first
                if (!await context.Database.CanConnectAsync(cancellationToken))
                {
                    throw new InvalidOperationException("Unable to connect to the database. Please check your credentials and try again.");
                }
                _logger.LogInformation("Database connection test successful");

                // Format the date parameter
                var formattedDate = startDate.ToString("yyyy-MM-dd");
                _logger.LogDebug("Formatted date parameter: {FormattedDate}", formattedDate);

                // Create parameter for the stored procedure
                var startDateParam = new SqlParameter("@StartDate", startDate);

                _logger.LogInformation("Executing stored procedure usp_HHR_Pool_30");
                
                // Execute the stored procedure and map the results
                var result = await context.Set<PoolDetail>()
                    .FromSqlRaw("EXEC [dbo].[usp_HHR_Pool_30] @StartDate", startDateParam)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Query completed. Retrieved {RecordCount} records", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing stored procedure usp_HHR_Pool_30. StartDate: {StartDate}. Error: {Error}", 
                    startDate, ex.Message);
                throw;
            }
        }
    }
}