namespace HHRReports.Desktop.Models
{
    public class PerformanceReport
    {
        public string? Location { get; set; }
        public string? Terminal { get; set; }
        public DateTime? Date { get; set; }
        public string? GrossRevenue { get; set; }
        public string? NetRevenue { get; set; }
        public string? TransactionCount { get; set; }
        public string? AverageWager { get; set; }
        public string? PayoutPercentage { get; set; }
        public string? HoldPercentage { get; set; }
        public string? Status { get; set; }
        public string? OperatingHours { get; set; }
        public string? RevenuePerHour { get; set; }
        public string? Category { get; set; }
        public string? Region { get; set; }
        public string? TaxAmount { get; set; }
        public string? CommissionAmount { get; set; }
    }
}