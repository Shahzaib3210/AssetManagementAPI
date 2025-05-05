namespace AssetManagementAPI.Models
{
    public class AssetInfo
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string DescriptionEstatePlan { get; set; }
        public decimal EstimateValue { get; set; }
        public decimal PurchaseCost { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime AsOfDate { get; set; }
        public bool IsFavorite { get; set; }
        public string StreetAddress { get; set; }
        public string StreetAddress2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string CountryCode { get; set; }
        public bool? UseZillow { get; set; }
        public string Neighborhood { get; set; }
        public string Slug { get; set; }
        public string Symbol { get; set; }
        public string CryptocurrencyName { get; set; }
        public decimal? Quantity { get; set; }
        public int? ManualAddType { get; set; }
        public int? ModelYear { get; set; }

        // Foreign key
        public int AssetId { get; set; }

        // Navigation property
        public Asset Asset { get; set; }
    }
}
