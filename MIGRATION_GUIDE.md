# HHR Reports Desktop Migration Guide

## Overview
This guide outlines the steps to complete the migration from Blazor Server to .NET MAUI Blazor Desktop application.

## Current Status
✅ Created .NET MAUI Blazor project structure
✅ Added necessary NuGet packages
✅ Created desktop-specific authentication service
✅ Created desktop-specific DbContext factory
✅ Copied models, services, and components

## Remaining Tasks

### 1. Update Service Namespaces
All services need namespace updates from `HHRReports` to `HHRReports.Desktop`:
- `PoolDetailService.cs`
- `TerminalDetailService.cs`
- `PerformanceReportService.cs`

### 2. Modify Services for Desktop
Each service needs to be updated to use `IDesktopDbContextFactory` instead of `IUserDbContextFactory`:

```csharp
// Old (Server version)
private readonly IUserDbContextFactory _contextFactory;
private readonly IHttpContextAccessor _httpContextAccessor;

// New (Desktop version)
private readonly IDesktopDbContextFactory _contextFactory;
// Remove IHttpContextAccessor - not needed for desktop
```

### 3. Update Service Methods
Remove HTTP context dependencies and simplify:

```csharp
// Old
public async Task<List<PoolDetail>> GetPoolDetailsAsync(DateTime startDate, string? authToken, CancellationToken cancellationToken)
{
    var httpContext = _httpContextAccessor.HttpContext;
    // ... authentication logic
    using var context = _contextFactory.CreateDbContext(httpContext);
}

// New
public async Task<List<PoolDetail>> GetPoolDetailsAsync(DateTime startDate, CancellationToken cancellationToken)
{
    using var context = _contextFactory.CreateDbContext();
    // Direct database access - auth handled by DesktopAuthenticationService
}
```

### 4. Update Blazor Components
Components need updates to remove web-specific features:

#### Remove or Adapt:
- `@inject IHttpContextAccessor` - Not needed
- `@inject NavigationManager` - Replace with MAUI navigation
- Cookie-based authentication - Use DesktopAuthenticationService
- JavaScript interop for auth tokens - Not needed

#### Login Component Changes:
```razor
@page "/login"
@inject IDesktopAuthenticationService AuthService
@inject NavigationManager Navigation

<!-- Add server and database input fields -->
<input @bind="server" placeholder="Server (e.g., hhr-sql.database.windows.net)" />
<input @bind="database" placeholder="Database (e.g., HHRPoolData)" />
<input @bind="username" placeholder="Username" />
<input type="password" @bind="password" placeholder="Password" />

@code {
    private async Task HandleLogin()
    {
        var success = await AuthService.LoginAsync(username, password, server, database);
        if (success)
        {
            Navigation.NavigateTo("/pool-details");
        }
    }
}
```

### 5. Update MainLayout
Create a desktop-appropriate layout without sidebar toggle (use standard MAUI menu):

```razor
@inherits LayoutComponentBase

<div class="page">
    <div class="sidebar">
        <NavMenu />
    </div>
    <main>
        <div class="top-row">
            <span>HHR Reports Desktop</span>
            @if (AuthService.IsAuthenticated)
            {
                <span>User: @AuthService.CurrentSession?.Username</span>
                <button @onclick="Logout">Logout</button>
            }
        </div>
        <article class="content">
            @Body
        </article>
    </main>
</div>
```

### 6. Configure App Settings
Create `appsettings.json` in the Desktop project root:

```json
{
  "DefaultConnection": {
    "Server": "hhr-sql.database.windows.net",
    "Database": "HHRPoolData"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 7. Update App.xaml.cs
Configure the main window:

```csharp
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new MainPage();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);
        window.Title = "HHR Reports Desktop";
        window.Width = 1400;
        window.Height = 900;
        return window;
    }
}
```

### 8. Fix Import Issues
Update all `using` statements:
- `using HHRReports.Models` → `using HHRReports.Desktop.Models`
- `using HHRReports.Services` → `using HHRReports.Desktop.Services`
- `using HHRReports.Data` → `using HHRReports.Desktop.Data`

### 9. Remove Web-Specific Features
- Remove SignalR references
- Remove cookie authentication
- Remove session management via HTTP context
- Remove render mode directives (`@rendermode InteractiveServer`)

### 10. Build and Test
```bash
cd ../HHRReports.Desktop
dotnet restore
dotnet build
dotnet run
```

## Security Considerations

### Connection String Storage
For production, implement secure storage:

```csharp
// Use Windows Credential Manager or similar
using Windows.Security.Credentials;

public void SaveCredentials(string username, string password)
{
    var vault = new PasswordVault();
    vault.Add(new PasswordCredential("HHRReports", username, password));
}

public PasswordCredential GetCredentials()
{
    var vault = new PasswordVault();
    return vault.Retrieve("HHRReports", username);
}
```

### Configuration Options
1. **Prompt for credentials each time** (most secure)
2. **Store encrypted in local settings** (convenient but less secure)
3. **Use Windows Authentication** (if supported by Azure SQL)

## Deployment

### Creating an Installer
1. Right-click the project in Visual Studio
2. Select "Publish"
3. Choose "Folder" as target
4. Configure as self-contained deployment
5. Use a tool like Inno Setup or WiX to create an installer

### Publishing Command Line
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Alternative Approach: Web API Backend

If direct database access from desktop proves problematic, consider:

1. **Create ASP.NET Core Web API**
   - Host on Azure App Service
   - Implement all database operations
   - Use JWT authentication

2. **Update Desktop App**
   - Call API endpoints instead of database
   - Store JWT tokens securely
   - Handle offline scenarios with caching

## Benefits of Desktop Version
- ✅ No browser required
- ✅ Native Windows experience
- ✅ Can work offline (with cached data)
- ✅ Faster UI response
- ✅ Access to local file system

## Limitations
- ⚠️ Must handle database connection security
- ⚠️ Need to manage application updates
- ⚠️ Windows-only (unless you target other platforms)
- ⚠️ Each client needs network access to Azure SQL

## Next Steps
1. Complete the service adaptations
2. Test database connectivity
3. Implement secure credential storage
4. Create installer for distribution
5. Consider implementing auto-update mechanism