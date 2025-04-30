namespace AssetManagementAPI.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<BalanceEntry> Balances { get; set; } = new();
    }
}
