# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

HHRReports is an ASP.NET Core 8.0 Blazor Server application for generating and displaying reports from a SQL Server database containing HHR (Historic Horse Racing) pool data. The application uses Entity Framework Core 9.0 for database operations.

## Tech Stack

- **Framework**: ASP.NET Core 8.0 with Blazor Server (Interactive Server render mode)
- **Database**: SQL Server (Azure SQL Database)
- **ORM**: Entity Framework Core 9.0 (with ADO.NET for complex scenarios)
- **Excel Export**: EPPlus library for Excel file generation
- **UI**: Bootstrap 5, custom CSS
- **JavaScript**: Minimal JS for table resizing and sidebar functionality
- **Authentication**: Custom per-user database authentication with session management

## Architecture

### Key Components

1. **Data Layer** (`Data/ApplicationDbContext.cs`)
   - EF Core DbContext managing database connections
   - Configured with SQL Server provider, retry logic, and 5-minute command timeout

2. **Services** (`Services/`)
   - Repository pattern implementation
   - `IPoolDetailService` interface with `PoolDetailService` implementation
   - `ITerminalDetailService` interface with `TerminalDetailService` implementation (uses ADO.NET for flexible column handling)
   - `IPerformanceReportService` interface with `PerformanceReportService` implementation
   - `IAuthenticationService` interface with `AuthenticationService` implementation
   - Services execute stored procedures and handle data retrieval with comprehensive error handling

3. **Models** (`Models/`)
   - POCOs representing database entities
   - `PoolDetail` and `TerminalDetail` models (keyless entities)
   - `PerformanceReport` model with string properties for flexible data handling
   - `UserSession` model for authentication state management

4. **Components/Pages** (`Components/Pages/`)
   - Blazor components using `@rendermode InteractiveServer`
   - `.razor` files with optional code-behind (`.razor.cs`)
   - Custom CSS per component (`.razor.css`)

## Build and Run Commands

```bash
# Build the project
dotnet build

# Run in development mode
dotnet run

# Run with hot reload
dotnet watch run

# Clean build artifacts
dotnet clean

# Restore NuGet packages
dotnet restore
```

## Database Operations

The application connects to Azure SQL Database and executes stored procedures:
- **Pool Details SP**: `usp_HHR_Pool_30` - Retrieves pool details for a 30-day period
- **Terminal Details SP**: `usp_HHR_TerminalDetail_30` - Retrieves terminal details for a 30-day period
- **Performance Report SP**: `usp_HHR_PerformanceReport` - Retrieves comprehensive performance metrics
- Connection strings are dynamically generated per user using their database credentials
- Database operations use async patterns with cancellation token support
- Per-user authentication validates access to stored procedures during login
- 20-minute command timeout for long-running stored procedures
- ADO.NET used for Terminal Details service to handle flexible column mapping

## Development Guidelines

### Service Registration
Services are registered in `Program.cs` using dependency injection:
- DbContext: AddDbContext with SQL Server configuration
- Services: AddScoped for repository implementations

### Blazor Component Patterns
- Use `@rendermode InteractiveServer` for interactive components
- MainLayout cannot use InteractiveServer due to RenderFragment Body parameter
- Implement `IDisposable` when using CancellationTokenSource
- Use `@inject` for dependency injection
- Handle loading states and error messages in UI

### Data Display Features
- Virtualized tables for large datasets using `<Virtualize>`
- Client-side search/filtering (Pool Details)
- Pagination controls (Pool Details)
- Responsive table layouts with Bootstrap
- Collapsible sidebar navigation with CSS-only toggle mechanism
- Dynamic content area expansion when sidebar is minimized
- Excel export functionality for all reports with proper formatting
- Execution timer display (SQL Server Management Studio style)
- Loading states and progress indicators
- Theme selection (White, Grey, Black)

### Error Handling
- Comprehensive logging using ILogger
- Try-catch blocks in service methods
- User-friendly error messages in UI
- Connection testing before query execution

## Important Notes

- **Security**: Per-user database authentication with dynamic connection strings
- **Performance**: Command timeout set to 20 minutes for long-running stored procedures
- **State Management**: Components use `StateHasChanged()` for UI updates during async operations
- **Cancellation**: Support for operation cancellation via CancellationToken throughout the stack
- **Authentication**: Custom authentication using auth tokens stored in cookies for Blazor Server SignalR
- **UI Layout**: CSS-only collapsible sidebar using checkbox toggle pattern for reliability
- **Hot Reload**: Complex UI changes may require app restart for proper application
- **ADO.NET**: Used for Terminal Details service to safely handle missing or optional database columns
- **Excel Export**: Uses EPPlus with proper formatting (currency, dates, percentages) and auto-fit columns
- **Percentage Formatting**: Use ToString("N2")% format instead of ToString("P2") to avoid multiplication by 100
- remember that we have 2 version of this report. cloud and destop. they should look the same and function the same.