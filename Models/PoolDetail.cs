namespace HHRReports.Desktop.Models
{
    public class PoolDetail
    {
        public int SiteID { get; set; }
        public string? SiteName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Pool { get; set; }
        public string? Zone { get; set; }
        public decimal Wagered { get; set; }
        public decimal TotalWon { get; set; }
        public decimal TotalCommission { get; set; }
        public decimal iPayoutPercent { get; set; }
        public decimal iTakeoutPercent { get; set; }
        public decimal OpenPariPoolValue { get; set; }
        public decimal TransferIn { get; set; }
        public decimal TransferOut { get; set; }
        public decimal DailyNetPoolChange { get; set; }
        public decimal ParimutuelPoolValue { get; set; }
        public decimal Allocated { get; set; }
        public string? strSiteState { get; set; }
    }
}
