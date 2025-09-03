using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.JSInterop;

namespace HHRReports.Desktop.Components.Pages;

public partial class PoolDetails
{
    private bool disposed;
    private string? errorMessage;
    private string? authToken;
    private bool showTimer;
    private string elapsedTime = "00:00:00";
    private string timerStatus = "Completed:";
    private System.Timers.Timer? executionTimer;
    private DateTime reportStartTime;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ApplyTheme();
        }
        
        // Always get the current auth token before operations (in case it changed)
        try
        {
            authToken = await JSRuntime.InvokeAsync<string?>("authHelpers.getCookie", "AuthToken");
            if (!string.IsNullOrEmpty(authToken))
            {
                Logger.LogDebug("Auth token retrieved from cookie for SignalR usage: {Token}", authToken);
            }
        }
        catch (Exception ex)
        {
            Logger.LogDebug("Could not get auth token: {Message}", ex.Message);
        }
        
        if (firstRender || (poolDetails != null && poolDetails.Any()))
        {
            // Small delay to ensure the table is fully rendered
            await Task.Delay(100);
            await JSRuntime.InvokeVoidAsync("initializeTableResizing");
        }
    }

    private async Task LoadReport()
    {
        errorMessage = null;
        loading = true;
        showTimer = true;
        reportStartTime = DateTime.Now;
        StartTimer();
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            // Get fresh auth token for each report generation
            try
            {
                authToken = await JSRuntime.InvokeAsync<string?>("authHelpers.getCookie", "AuthToken");
                Logger.LogDebug("Fresh auth token retrieved for report generation: {Token}", authToken);
            }
            catch (Exception ex)
            {
                Logger.LogDebug("Could not get auth token: {Message}", ex.Message);
            }

            // Pass auth token for SignalR requests
            if (!string.IsNullOrEmpty(authToken))
            {
                try
                {
                    poolDetails = await PoolService.GetPoolDetailsAsync(startDate, authToken, _cancellationTokenSource.Token);
                    timerStatus = "Completed:";
                }
                catch (UnauthorizedAccessException)
                {
                    // Token is stale, clear it and redirect to login
                    Logger.LogWarning("Auth token is stale, clearing and redirecting to login");
                    await JSRuntime.InvokeVoidAsync("authHelpers.deleteCookie", "AuthToken");
                    Navigation.NavigateTo("/login", forceLoad: true);
                    return;
                }
            }
            else
            {
                poolDetails = await PoolService.GetPoolDetailsAsync(startDate, _cancellationTokenSource.Token);
                timerStatus = "Completed:";
            }
        }
        catch (OperationCanceledException)
        {
            errorMessage = "Report generation was cancelled.";
            timerStatus = "Cancelled:";
        }
        catch (SqlException ex) when (ex.Number == -2)
        {
            errorMessage = "The report is taking longer than expected. Please try again or contact support if the issue persists.";
            Logger.LogError(ex, "Timeout occurred while generating report");
            timerStatus = "Timeout:";
        }
        catch (Exception ex)
        {
            errorMessage = "An error occurred while generating the report. Please try again or contact support if the issue persists.";
            Logger.LogError(ex, "Error occurred while generating report");
            timerStatus = "Failed:";
        }
        finally
        {
            loading = false;
            StopTimer();
            // Keep showTimer as true to show the final time
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }
    
    private void StartTimer()
    {
        executionTimer?.Dispose();
        executionTimer = new System.Timers.Timer(100); // Update every 100ms
        executionTimer.Elapsed += (sender, e) =>
        {
            var elapsed = DateTime.Now - reportStartTime;
            elapsedTime = $"{elapsed.Hours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}";
            InvokeAsync(StateHasChanged);
        };
        executionTimer.Start();
    }
    
    private void StopTimer()
    {
        executionTimer?.Stop();
        executionTimer?.Dispose();
        executionTimer = null;
    }

    void IDisposable.Dispose()
    {
        disposed = true;
        _cancellationTokenSource?.Dispose();
        StopTimer();
    }
}