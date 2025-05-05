namespace AssetManagementAPI.Models
{
    public class Holding
    {
        public int Id { get; set; }
        public string MajorClass { get; set; }
        public string MinorClass { get; set; }
        public decimal Value { get; set; }

        // Foreign key
        public int AssetId { get; set; }

        // Navigation property
        public Asset Asset { get; set; }
    }
}
