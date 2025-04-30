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

        public AssetsController(AssetDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddAssets([FromBody] List<Asset> assets)
        {
            try
            {
                if(assets != null && assets.Count > 0)
                {
                    foreach (var asset in assets)
                    {
                        _context.Assets.Add(asset);
                    }
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("No assets provided.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving assets to the database", ex);
            }
        }

        [HttpGet("asof")]
        public IActionResult GetAssetsAsOf([FromQuery] DateTime date)
        {
            var result = _context.Assets
                .Select(a => new
                {
                    a.Id,
                    a.Name,
                    a.Type,
                    Balance = a.Balances
                        .Where(b => b.BalanceAsOf <= date)
                        .OrderByDescending(b => b.BalanceAsOf)
                        .FirstOrDefault()
                })
                .Where(a => a.Balance != null)
                .ToList();

            return Ok(result);
        }
    }
}
