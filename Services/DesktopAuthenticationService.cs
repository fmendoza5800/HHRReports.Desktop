using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using HHRReports.Desktop.Models;
using HHRReports.Desktop.Data;
using Microsoft.Extensions.Configuration;

namespace HHRReports.Desktop.Services
{
    public interface IDesktopAuthenticationService
    {
        Task<bool> LoginAsync(string username, string password, string server, string database);
        void Logout();
        bool IsAuthenticated { get; }
        string? Username { get; }
        DateTime LoginTime { get; }
        UserSession? CurrentSession { get; }
        string GetConnectionString();
    }

    public class DesktopAuthenticationService : IDesktopAuthenticationService
    {
        private readonly ILogger<DesktopAuthenticationService> _logger;
        private readonly IConfiguration _configuration;
        private UserSession? _currentSession;
        private string? _connectionString;

        public DesktopAuthenticationService(ILogger<DesktopAuthenticationService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public bool IsAuthenticated => _currentSession != null;
        public string? Username => _currentSession?.Username;
        public DateTime LoginTime => _currentSession?.LoginTime ?? DateTime.MinValue;
        public UserSession? CurrentSession => _currentSession;

        public async Task<bool> LoginAsync(string username, string password, string server, string database)
        {
            try
            {
                _logger.LogInformation("Attempting login for user: {Username}", username);

                // Build connection string with user credentials
                var connectionBuilder = new SqlConnectionStringBuilder
                {
                    DataSource = server,
                    InitialCatalog = database,
                    UserID = username,
                    Password = password,
                    TrustServerCertificate = true,
                    Encrypt = true,
                    ConnectTimeout = 30,
                    CommandTimeout = 1200 // 20 minutes
                };

                _connectionString = connectionBuilder.ConnectionString;

                // Test the connection and check permissions
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    _logger.LogInformation("Database connection test successful for user: {Username}", username);

                    // Check if user has access to required stored procedures
                    var checkPoolSP = @"
                        SELECT CASE 
                            WHEN EXISTS (
                                SELECT 1 
                                FROM sys.procedures p
                                JOIN sys.database_permissions dp ON dp.major_id = p.object_id
                                WHERE p.name = 'usp_HHR_Pool_30' 
                                AND dp.permission_name = 'EXECUTE'
                                AND dp.state = 'G'
                                AND USER_NAME(dp.grantee_principal_id) = USER_NAME()
                            ) THEN 1
                            ELSE 0
                        END";

                    var checkTerminalSP = @"
                        SELECT CASE 
                            WHEN EXISTS (
                                SELECT 1 
                                FROM sys.procedures p
                                JOIN sys.database_permissions dp ON dp.major_id = p.object_id
                                WHERE p.name = 'usp_HHR_TerminalDetail_30' 
                                AND dp.permission_name = 'EXECUTE'
                                AND dp.state = 'G'
                                AND USER_NAME(dp.grantee_principal_id) = USER_NAME()
                            ) THEN 1
                            ELSE 0
                        END";

                    var checkPerformanceSP = @"
                        SELECT CASE 
                            WHEN EXISTS (
                                SELECT 1 
                                FROM sys.procedures p
                                JOIN sys.database_permissions dp ON dp.major_id = p.object_id
                                WHERE p.name = 'usp_HHR_PerformanceReport' 
                                AND dp.permission_name = 'EXECUTE'
                                AND dp.state = 'G'
                                AND USER_NAME(dp.grantee_principal_id) = USER_NAME()
                            ) THEN 1
                            ELSE 0
                        END";

                    bool canExecutePoolSP = false;
                    bool canExecuteTerminalSP = false;
                    bool canExecutePerformanceSP = false;

                    using (var command = new SqlCommand(checkPoolSP, connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        canExecutePoolSP = Convert.ToBoolean(result);
                    }

                    using (var command = new SqlCommand(checkTerminalSP, connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        canExecuteTerminalSP = Convert.ToBoolean(result);
                    }

                    using (var command = new SqlCommand(checkPerformanceSP, connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        canExecutePerformanceSP = Convert.ToBoolean(result);
                    }

                    _logger.LogInformation("User {Username} permissions - Pool: {Pool}, Terminal: {Terminal}, Performance: {Performance}",
                        username, canExecutePoolSP, canExecuteTerminalSP, canExecutePerformanceSP);

                    if (!canExecutePoolSP && !canExecuteTerminalSP && !canExecutePerformanceSP)
                    {
                        _logger.LogWarning("User {Username} does not have execute permissions on any required stored procedures", username);
                        return false;
                    }

                    // Create session
                    _currentSession = new UserSession
                    {
                        Username = username,
                        LoginTime = DateTime.Now,
                        LastActivity = DateTime.Now,
                        ConnectionString = _connectionString,
                        AuthToken = Guid.NewGuid().ToString()
                    };

                    _logger.LogInformation("Login successful for user: {Username}", username);
                    return true;
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection failed for user: {Username}", username);
                _currentSession = null;
                _connectionString = null;
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for user: {Username}", username);
                _currentSession = null;
                _connectionString = null;
                return false;
            }
        }

        public void Logout()
        {
            _logger.LogInformation("User logged out: {Username}", _currentSession?.Username);
            _currentSession = null;
            _connectionString = null;
        }

        public string GetConnectionString()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("User is not authenticated");
            
            return _connectionString;
        }
    }
}