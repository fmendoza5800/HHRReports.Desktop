namespace HHRReports.Desktop.Models
{
    public class TerminalDetail
    {
        public int SiteID { get; set; }
        public string? SiteName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? strGameDescription { get; set; }
        public string? Pool { get; set; }
        public string? Device { get; set; }
        public decimal iDenom { get; set; }
        public decimal CashIn { get; set; }
        public decimal TicketIn { get; set; }
        public decimal TicketOut { get; set; }
        public decimal CanceledCredit { get; set; }
        public decimal HandPayWins { get; set; }
        public decimal TotalMoneyCashedOut { get; set; }
        public decimal GTBalance { get; set; }
        public decimal TotalMoneyIn { get; set; }
        public decimal TotalMoneyOut { get; set; }
        public decimal Balance { get; set; }
        public decimal Played { get; set; }
        public decimal TotalMoneyWon { get; set; }
        public decimal iNonCashIn { get; set; }
        public decimal iNonCashOut { get; set; }
        public decimal Hold { get; set; }
        public decimal Wagered { get; set; }
        public decimal TotalWon { get; set; }
        public decimal TotalCommission { get; set; }
        public decimal TerminalCommission { get; set; }
        public string? AssetNumber { get; set; }
    }
}
