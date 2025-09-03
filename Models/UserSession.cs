using System.ComponentModel.DataAnnotations;

namespace HHRReports.Desktop.Models
{
    public class UserSession
    {
        public string Username { get; set; } = string.Empty;
        public string Server { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public DateTime LoginTime { get; set; }
        public DateTime LastActivity { get; set; }
        public bool IsAuthenticated { get; set; }
        
        // Connection string for database access
        public string? ConnectionString { get; set; }
        public string? AuthToken { get; set; }
        
        // Encrypted connection string - never store plain text credentials
        public string EncryptedConnectionString { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters")]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password is required")]
        [StringLength(200, ErrorMessage = "Password cannot exceed 200 characters")]
        public string Password { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Server is required")]
        public string Server { get; set; } = "tcp:hhr-sql.database.windows.net,1433";
        
        [Required(ErrorMessage = "Database is required")]
        public string Database { get; set; } = "HHRPoolData";
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public UserSession? Session { get; set; }
    }
}