using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeedsToTest.API.Data.Context;
using NeedsToTest.API.Data.Entities;

namespace NeedsToTest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly MyAmazonDbContext _context;

        public ProductController(MyAmazonDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? category = null)
        {
            ICollection<Product> products;
            
            if (!string.IsNullOrEmpty(category))
            {
                products = await _context.Products.Where(p => p.Categories.Contains(category)).ToListAsync();
                
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
