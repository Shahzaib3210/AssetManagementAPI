namespace AssetManagementAPI.Models
{
    public class BalanceEntry
    {
        public int Id { get; set; }
        public DateTime BalanceAsOf { get; set; }
        public decimal Amount { get; set; }
        public decimal? BalanceCostBasis { get; set; }
        public string BalanceCostFrom { get; set; }
        public string BalanceFrom { get; set; }
        public decimal? BalancePrice { get; set; }
        public string BalancePriceFrom { get; set; }
        public decimal? BalanceQuantityCurrent { get; set; }

        // Foreign key
        public int AssetId { get; set; }

        // Navigation property
        public Asset Asset { get; set; }
    }
}
