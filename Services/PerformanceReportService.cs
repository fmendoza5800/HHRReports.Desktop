using Microsoft.EntityFrameworkCore;
using HHRReports.Desktop.Models;
using HHRReports.Desktop.Data;
using Microsoft.Data.SqlClient;

namespace HHRReports.Desktop.Services
{
    public interface IPerformanceReportService
    {
        Task<List<PerformanceReport>> GetPerformanceReportAsync(DateTime endDate, CancellationToken cancellationToken = default);
        Task<List<PerformanceReport>> GetPerformanceReportsAsync(DateTime endDate, string? authToken, CancellationToken cancellationToken = default);
        Task<List<PerformanceReport>> GetPerformanceReportsAsync(DateTime endDate, CancellationToken cancellationToken = default);
    }

    public class PerformanceReportService : IPerformanceReportService
    {
        private readonly IDesktopDbContextFactory _contextFactory;
        private readonly ILogger<PerformanceReportService> _logger;

        public PerformanceReportService(
            IDesktopDbContextFactory contextFactory,
            ILogger<PerformanceReportService> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<List<PerformanceReport>> GetPerformanceReportAsync(DateTime endDate, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Starting GetPerformanceReportAsync for end date: {EndDate}", endDate);

                using var context = _contextFactory.CreateDbContext();
                
                // Test database connection first
                if (!await context.Database.CanConnectAsync(cancellationToken))
                {
                    throw new InvalidOperationException("Unable to connect to the database. Please check your credentials and try again.");
                }
                _logger.LogInformation("Database connection test successful");

                // Format the date parameter
                var formattedDate = endDate.ToString("yyyy-MM-dd");
                _logger.LogDebug("Formatted date parameter: {FormattedDate}", formattedDate);

                // Create parameter for the stored procedure
                var endDateParam = new SqlParameter("@EndDate", endDate);

                _logger.LogInformation("Executing stored procedure usp_HHR_PerformanceReport");
                
                // Execute the stored procedure and map the results
                var result = await context.Set<PerformanceReport>()
                    .FromSqlRaw("EXEC [dbo].[usp_HHR_PerformanceReport] @EndDate", endDateParam)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Query completed. Retrieved {RecordCount} records", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing stored procedure usp_HHR_PerformanceReport. EndDate: {EndDate}. Error: {Error}", 
                    endDate, ex.Message);
                throw;
            }
        }

        public async Task<List<PerformanceReport>> GetPerformanceReportsAsync(DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await GetPerformanceReportsAsync(endDate, null, cancellationToken);
        }

        public async Task<List<PerformanceReport>> GetPerformanceReportsAsync(DateTime endDate, string? authToken, CancellationToken cancellationToken = default)
        {
            // For desktop version, auth token is ignored and we use the existing method
            return await GetPerformanceReportAsync(endDate, cancellationToken);
        }
    }
}