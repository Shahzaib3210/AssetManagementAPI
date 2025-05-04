using AssetManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;

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

        // Endpoint to add a list of assets to the database
        [HttpPost("AddAssets")]
        public async Task<IActionResult> AddAssets([FromBody] List<Asset> assets)
        {
            try
            {
                // Check if the provided list of assets is not null and contains at least one asset
                if (assets != null && assets.Count > 0)
                {
                    // Loop through each asset and add it to the database context
                    foreach (var asset in assets)
                    {
                        _context.Assets.Add(asset);
                    }

                    // Save all changes to the database asynchronously
                    await _context.SaveChangesAsync();

                    // Return a 200 OK response if successful
                    return Ok();
                }
                else
                {
                    // Return a 400 Bad Request response if no assets were provided
                    return BadRequest("No assets provided.");
                }
            }
            catch (Exception ex)
            {
                // Throw a new exception with additional context if an error occurs
                throw new Exception("AddAssets - Error saving assets to the database", ex);
            }
        }

        // Endpoint to retrieve assets and their most recent balance as of a specific date
        [HttpGet("GetAssetsAsOfDate")]
        public IActionResult GetAssetsAsOf([FromQuery] DateTime date)
        {
            try
            {
                // Query the database for assets that have at least one balance entry on or before the specified date
                var result = _context.Assets
                    .Where(a => a.Balances.Any(b => b.BalanceAsOf <= date)) // Filter assets with balances up to the given date
                    .Select(a => new
                    {
                        a.Id, // Asset ID
                        a.Name, // Asset name
                        a.Type, // Asset type
                        Balance = a.Balances
                            .Where(b => b.BalanceAsOf <= date) // Filter balances up to the given date
                            .OrderByDescending(b => b.BalanceAsOf) // Order balances by the most recent date
                            .Select(b => new
                            {
                                b.BalanceAsOf, // Date of the balance
                                b.Amount // Balance amount
                            })
                            .FirstOrDefault() // Select the most recent balance
                    })
                    .ToList();

                // Return the result as a 200 OK response
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Throw a new exception with additional context if an error occurs
                throw new Exception("GetAssetsAsOf - Error getting the assets from database", ex);
            }
        }

        // Endpoint to retrieve the historical balances of a specific asset
        [HttpGet("GetHistoricalBalances")]
        public IActionResult GetHistoricalBalances([FromQuery] int assetId)
        {
            try
            {
                // Query the database for the asset with the specified ID and its balances
                var asset = _context.Assets
                    .Where(a => a.Id == assetId) // Filter by asset ID
                    .Select(a => new
                    {
                        a.Id, // Asset ID
                        a.Name, // Asset name
                        a.Type, // Asset type
                        Balances = a.Balances
                            .OrderByDescending(b => b.BalanceAsOf) // Order balances by date descending
                            .Select(b => new
                            {
                                b.BalanceAsOf, // Date of the balance
                                b.Amount // Balance amount
                            })
                    })
                    .FirstOrDefault();

                // If the asset is not found, return a 404 Not Found response
                if (asset == null)
                {
                    return NotFound($"Asset with ID {assetId} not found.");
                }

                // Return the asset and its balances as a 200 OK response
                return Ok(asset);
            }
            catch (Exception ex)
            {
                // Throw a new exception with additional context if an error occurs
                throw new Exception("GetHistoricalBalances - Error getting the historial balances from database", ex);
            }
        }
    }
}
