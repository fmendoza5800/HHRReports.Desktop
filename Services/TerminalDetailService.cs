using Microsoft.EntityFrameworkCore;
using HHRReports.Desktop.Models;
using HHRReports.Desktop.Data;
using Microsoft.Data.SqlClient;

namespace HHRReports.Desktop.Services
{
    public interface ITerminalDetailService
    {
        Task<List<TerminalDetail>> GetTerminalDetailsAsync(DateTime endDate, CancellationToken cancellationToken = default);
        Task<List<TerminalDetail>> GetTerminalDetailsAsync(DateTime endDate, string? authToken, CancellationToken cancellationToken = default);
    }

    public class TerminalDetailService : ITerminalDetailService
    {
        private readonly IDesktopDbContextFactory _contextFactory;
        private readonly ILogger<TerminalDetailService> _logger;

        public TerminalDetailService(
            IDesktopDbContextFactory contextFactory, 
            ILogger<TerminalDetailService> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<List<TerminalDetail>> GetTerminalDetailsAsync(DateTime endDate, CancellationToken cancellationToken = default)
        {
            // Call the overloaded method with null authToken
            return await GetTerminalDetailsAsync(endDate, null, cancellationToken);
        }

        public async Task<List<TerminalDetail>> GetTerminalDetailsAsync(DateTime endDate, string? authToken, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Starting GetTerminalDetailsAsync for end date: {EndDate}", endDate);

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
                
                _logger.LogInformation("Executing stored procedure usp_HHR_TerminalDetail_30");
                
                // Use ADO.NET directly for more flexibility
                var terminalDetails = new List<TerminalDetail>();
                
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "usp_HHR_TerminalDetail_30";
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandTimeout = 1200; // 20 minutes
                    
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@EndDate";
                    parameter.Value = endDate;
                    command.Parameters.Add(parameter);
                    
                    await context.Database.OpenConnectionAsync(cancellationToken);
                    
                    using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        while (await reader.ReadAsync(cancellationToken))
                        {
                            var detail = new TerminalDetail();
                            
                            // Helper function to safely get column values
                            int GetOrdinalSafe(string columnName)
                            {
                                try
                                {
                                    return reader.GetOrdinal(columnName);
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    return -1;
                                }
                            }
                            
                            T GetValueSafe<T>(string columnName, T defaultValue = default(T))
                            {
                                var ordinal = GetOrdinalSafe(columnName);
                                if (ordinal >= 0 && !reader.IsDBNull(ordinal))
                                {
                                    return (T)reader.GetValue(ordinal);
                                }
                                return defaultValue;
                            }
                            
                            string GetStringSafe(string columnName)
                            {
                                var ordinal = GetOrdinalSafe(columnName);
                                return ordinal >= 0 && !reader.IsDBNull(ordinal) ? reader.GetString(ordinal) : null;
                            }
                            
                            decimal GetDecimalSafe(string columnName)
                            {
                                var ordinal = GetOrdinalSafe(columnName);
                                return ordinal >= 0 && !reader.IsDBNull(ordinal) ? reader.GetDecimal(ordinal) : 0;
                            }
                            
                            int GetIntSafe(string columnName)
                            {
                                var ordinal = GetOrdinalSafe(columnName);
                                return ordinal >= 0 && !reader.IsDBNull(ordinal) ? reader.GetInt32(ordinal) : 0;
                            }
                            
                            DateTime? GetDateTimeSafe(string columnName)
                            {
                                var ordinal = GetOrdinalSafe(columnName);
                                return ordinal >= 0 && !reader.IsDBNull(ordinal) ? reader.GetDateTime(ordinal) : null;
                            }
                            
                            // Populate the object safely
                            detail.SiteID = GetIntSafe("SiteID");
                            detail.SiteName = GetStringSafe("SiteName");
                            detail.StartDate = GetValueSafe<DateTime>("StartDate");
                            detail.EndDate = GetDateTimeSafe("EndDate") ?? GetValueSafe<DateTime>("StartDate");
                            detail.strGameDescription = GetStringSafe("strGameDescription");
                            detail.Pool = GetStringSafe("Pool");
                            detail.Device = GetStringSafe("Device");
                            detail.iDenom = GetDecimalSafe("iDenom");
                            detail.CashIn = GetDecimalSafe("CashIn");
                            detail.TicketIn = GetDecimalSafe("TicketIn");
                            detail.TicketOut = GetDecimalSafe("TicketOut");
                            detail.CanceledCredit = GetDecimalSafe("CanceledCredit");
                            detail.HandPayWins = GetDecimalSafe("HandPayWins");
                            detail.TotalMoneyCashedOut = GetDecimalSafe("TotalMoneyCashedOut");
                            detail.GTBalance = GetDecimalSafe("GTBalance");
                            detail.TotalMoneyIn = GetDecimalSafe("TotalMoneyIn");
                            detail.TotalMoneyOut = GetDecimalSafe("TotalMoneyOut");
                            detail.Balance = GetDecimalSafe("Balance");
                            detail.Played = GetDecimalSafe("Played");
                            detail.TotalMoneyWon = GetDecimalSafe("TotalMoneyWon");
                            detail.iNonCashIn = GetDecimalSafe("iNonCashIn");
                            detail.iNonCashOut = GetDecimalSafe("iNonCashOut");
                            detail.Hold = GetDecimalSafe("Hold");
                            detail.Wagered = GetDecimalSafe("Wagered");
                            detail.TotalWon = GetDecimalSafe("TotalWon");
                            detail.TotalCommission = GetDecimalSafe("TotalCommission");
                            detail.TerminalCommission = GetDecimalSafe("TerminalCommission");
                            detail.AssetNumber = GetStringSafe("AssetNumber");
                            
                            terminalDetails.Add(detail);
                        }
                    }
                }
                
                _logger.LogInformation("Query completed. Retrieved {RecordCount} records", terminalDetails.Count);
                return terminalDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing stored procedure usp_HHR_TerminalDetail_30. EndDate: {EndDate}. Error: {Error}", 
                    endDate, ex.Message);
                throw;
            }
        }
    }
}