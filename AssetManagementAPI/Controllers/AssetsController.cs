using AssetManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AssetManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly AssetDbContext _context;

        // Constructor to initialize the database context
        public AssetsController(AssetDbContext context)
        {
            _context = context;
        }

        // Endpoint to add multiple assets to the database
        [HttpPost("AddAssets")]
        public async Task<IActionResult> AddAssets([FromBody] JsonElement jsonData)
        {
            try
            {
                // Lists to hold parsed data for bulk insertion
                var assets = new List<Asset>();
                var assetInfos = new List<AssetInfo>();
                var holdings = new List<Holding>();

                // Iterate through each asset in the input JSON array
                foreach (var assetElement in jsonData.EnumerateArray())
                {
                    // Extract and validate the "assetInfo" JSON string
                    var assetInfoJson = assetElement.GetProperty("assetInfo").GetString();
                    if (string.IsNullOrEmpty(assetInfoJson))
                    {
                        return BadRequest("AssetInfo JSON is missing or invalid.");
                    }

                    // Deserialize the "assetInfo" JSON into an AssetInfo object
                    var assetInfo = System.Text.Json.JsonSerializer.Deserialize<AssetInfo>(assetInfoJson, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    if (assetInfo == null)
                    {
                        return BadRequest("Failed to deserialize AssetInfo.");
                    }

                    // Extract required fields from the JSON object
                    var assetId = assetElement.GetProperty("assetId").GetString();
                    var nickname = assetElement.GetProperty("nickname").GetString();
                    var assetInfoType = assetElement.GetProperty("assetInfoType").GetString();
                    var wealthAssetType = assetElement.GetProperty("wealthAssetType").GetString();
                    var creationDateString = assetElement.GetProperty("creationDate").GetString();
                    var balanceAsOfString = assetElement.GetProperty("balanceAsOf").GetString();
                    var balanceFrom = assetElement.GetProperty("balanceFrom").GetString();

                    // Validate required fields
                    if (string.IsNullOrEmpty(assetId) || string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(assetInfoType) ||
                        string.IsNullOrEmpty(wealthAssetType) || string.IsNullOrEmpty(creationDateString) || string.IsNullOrEmpty(balanceAsOfString) ||
                        string.IsNullOrEmpty(balanceFrom))
                    {
                        return BadRequest("One or more required fields are missing or invalid.");
                    }

                    // Create a new Asset object and populate its properties
                    var asset = new Asset
                    {
                        AssetId = assetId,
                        Nickname = nickname,
                        AssetInfo = string.Empty, // Assign an empty string to avoid nullability issues
                        AssetInfoType = assetInfoType,
                        WealthAssetType = wealthAssetType,
                        IncludeInNetWorth = assetElement.GetProperty("includeInNetWorth").GetBoolean(),
                        IsActive = assetElement.GetProperty("isActive").GetBoolean(),
                        CreationDate = DateTime.Parse(creationDateString),
                        ModificationDate = assetElement.TryGetProperty("modificationDate", out var modDate) && !string.IsNullOrEmpty(modDate.GetString())
                            ? DateTime.Parse(modDate.GetString()!)
                            : null,
                        Balances = new List<BalanceEntry>
                        {
                            new BalanceEntry
                            {
                                BalanceAsOf = DateTime.Parse(balanceAsOfString),
                                Amount = assetElement.GetProperty("balanceCurrent").GetDecimal(),
                                BalanceFrom = balanceFrom,
                                BalanceCostBasis = assetElement.TryGetProperty("balanceCostBasis", out var costBasis) ? costBasis.GetDecimal() : null
                            }
                        }
                    };

                    // Add the asset to the list for bulk insertion
                    assets.Add(asset);

                    // Link the deserialized AssetInfo to the asset and add it to the list
                    assetInfo.Asset = asset;
                    assetInfos.Add(assetInfo);

                    // Process holdings if they exist in the JSON object
                    if (assetElement.TryGetProperty("holdings", out var holdingsArray) && holdingsArray.ValueKind == JsonValueKind.Object)
                    {
                        var holdingsData = System.Text.Json.JsonSerializer.Deserialize<HoldingsData>(holdingsArray.GetRawText());
                        if (holdingsData != null)
                        {
                            foreach (var majorClass in holdingsData.majorAssetClasses)
                            {
                                foreach (var assetClass in majorClass.assetClasses)
                                {
                                    holdings.Add(new Holding
                                    {
                                        MajorClass = majorClass.majorClass ?? string.Empty, // Ensure non-null value
                                        MinorClass = assetClass.minorAssetClass ?? string.Empty, // Ensure non-null value
                                        Value = assetClass.value,
                                        Asset = asset
                                    });
                                }
                            }
                        }
                    }
                }

                // Add all parsed data to the database context
                _context.Assets.AddRange(assets);
                _context.AssetInfos.AddRange(assetInfos);
                _context.Holdings.AddRange(holdings);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Return success response with the count of imported assets
                return Ok($"{assets.Count} assets imported successfully");
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions
                return StatusCode(500, $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                return StatusCode(500, $"Error importing assets: {ex.Message}");
            }
        }

        // Helper classes for deserialization of holdings data
        public class HoldingsData
        {
            public List<MajorAssetClass> majorAssetClasses { get; set; } = new();
        }

        public class MajorAssetClass
        {
            public string majorClass { get; set; }
            public List<AssetClass> assetClasses { get; set; } = new();
        }

        public class AssetClass
        {
            public string minorAssetClass { get; set; }
            public decimal value { get; set; }
        }

        // Endpoint to retrieve assets as of a specific date
        [HttpGet("GetAssetsAsOfDate")]
        public IActionResult GetAssetsAsOfDate([FromQuery] DateTime date)
        {
            try
            {
                // Query to fetch assets with their latest balances as of the given date
                var result = _context.Assets
                    .Include(a => a.Balances)
                    .Select(a => new
                    {
                        a.AssetId,
                        a.Nickname,
                        a.WealthAssetType,
                        a.CreationDate,
                        LatestBalance = a.Balances
                            .Where(b => b.BalanceAsOf.Date <= date.Date) // Filter balances on or before the date
                            .OrderByDescending(b => b.BalanceAsOf) // Get the most recent balance
                            .FirstOrDefault()
                    })
                    .Where(a => a.LatestBalance != null) // Exclude assets with no valid balances
                    .OrderByDescending(a => a.LatestBalance.BalanceAsOf) // Sort by the latest balance date
                    .Take(1) // Take only the top asset with the latest balance
                    .ToList();

                // Return the result as a JSON response
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                return StatusCode(500, $"Error retrieving assets: {ex.Message}");
            }
        }

        // Endpoint to retrieve historical balances for a specific asset
        [HttpGet("GetHistoricalBalances")]
        public IActionResult GetHistoricalBalances([FromQuery] string assetId)
        {
            try
            {
                // Fetch the asset and its balances from the database
                var asset = _context.Assets
                    .Include(a => a.Balances)
                    .FirstOrDefault(a => a.AssetId == assetId);

                // Return 404 if the asset is not found
                if (asset == null)
                {
                    return NotFound($"Asset with ID {assetId} not found.");
                }

                // Prepare the result with historical balances
                var result = new
                {
                    asset.AssetId,
                    asset.Nickname,
                    asset.WealthAssetType,
                    Balances = asset.Balances
                        .OrderByDescending(b => b.BalanceAsOf)
                        .Select(b => new
                        {
                            b.BalanceAsOf,
                            Amount = b.Amount,
                            BalanceCostBasis = b.BalanceCostBasis.HasValue ? b.BalanceCostBasis.Value : (decimal?)null
                        })
                };

                // Return the result as a JSON response
                return Ok(result);
            }
            catch (System.Data.SqlTypes.SqlNullValueException ex)
            {
                // Handle null value exceptions
                return BadRequest($"Null value encountered: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                return StatusCode(500, $"Error retrieving historical balances: {ex.Message}");
            }
        }
    }
}