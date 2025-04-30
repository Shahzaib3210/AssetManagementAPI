namespace AssetManagementAPI.Models
{
    public class BalanceEntry
    {
        public int Id { get; set; }
        public DateTime BalanceAsOf { get; set; }
        public decimal Amount { get; set; }
    }
}
