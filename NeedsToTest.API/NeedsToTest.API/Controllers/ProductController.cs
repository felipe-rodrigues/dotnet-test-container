using Bogus.Extensions.UnitedKingdom;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.IO;
using NeedsToTest.API.Data.Context;
using NeedsToTest.API.Data.Entities;
using StackExchange.Redis;
using System.Text.Json;

namespace NeedsToTest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly MyAmazonDbContext _context;
        private readonly IDatabase _redis;

        public ProductController(MyAmazonDbContext context, IDatabase redis)
        {
            _context = context;
            _redis = redis;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? category = null)
        {
            ICollection<Product> products;
            
            if (!string.IsNullOrEmpty(category))
            {

                var keyCategory = $"category:{category}";
                if(_redis.KeyExists(keyCategory))
                {
                    var dataCache = await _redis.StringGetAsync(keyCategory);
                    var serializedProducts = JsonSerializer.Deserialize<List<Product>>(dataCache);
                    products = serializedProducts;
                }
                else
                {
                    products = await _context.Products.Where(p => p.Categories.Contains(category)).ToListAsync();
                    var serializedProducts = JsonSerializer.Serialize(products);
                    await _redis.StringSetAsync(keyCategory, serializedProducts);
                }
               
            }
            else
            {
                products = await _context.Products.ToListAsync();
            }
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Product product)
        {
            product.Id = System.Guid.NewGuid().ToString();
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return Ok(product);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if(product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

    }
}
