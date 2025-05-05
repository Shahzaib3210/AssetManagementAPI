namespace AssetManagementAPI.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string AssetId { get; set; }
        public string AssetDescription { get; set; }
        public string AssetInfo { get; set; }
        public string AssetInfoType { get; set; }
        public string AssetName { get; set; }
        public string Nickname { get; set; }
        public string PrimaryAssetCategory { get; set; }
        public string WealthAssetType { get; set; }
        public string CognitoId { get; set; }
        public string Wid { get; set; }
        public string UserInstitutionId { get; set; }
        public int? InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public bool IncludeInNetWorth { get; set; }
        public bool IsActive { get; set; }
        public bool? IsAsset { get; set; }
        public bool? IsFavorite { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public DateTime? LastUpdateAttempt { get; set; }
        public string LogoName { get; set; }
        public string Status { get; set; }
        public string StatusCode { get; set; }
        public string VendorResponseType { get; set; }
        public List<BalanceEntry> Balances { get; set; }
        public List<Holding> Holdings { get; set; }
        public List<AssetInfo> AssetInfos { get; set; }
    }
}
