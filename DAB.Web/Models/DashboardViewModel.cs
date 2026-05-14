namespace DAB.Web.Models
{
    public class DashboardViewModel
    {
        public int TotalAccounts { get; set; }
        public decimal TotalBalance { get; set; }
        public int TransactionsCount { get; set; }
        public int ConnectedAtms { get; set; }
        public List<Transaction> RecentTransactions { get; set; } = new();
    }
}