using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IDistributedCache _cache, ApplicationDbContext _dbContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var memoryCache = await _cache.GetStringAsync("ProductsCache");
            if (memoryCache == null)
            {
                var products = await _dbContext.Product.ToListAsync();
                await _cache.SetStringAsync("ProductsCache", JsonSerializer.Serialize(products));
                return Ok(products);
            }

            return Ok(JsonSerializer.Deserialize<List<Product>>(memoryCache));
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Product product)
        {
            await _dbContext.Product.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            await _cache.SetStringAsync("ProductsCache", JsonSerializer.Serialize(_dbContext.Product));
            return Ok("Added");
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Product product)
        {
            var get = await _dbContext.Product.FindAsync(product.Id);
            if (get == null)
            {
                return NotFound("404");
            }

            get.Name = product.Name;
            get.Price = product.Price;
            get.Description = product.Description;
            _dbContext.Product.Update(get);
            await _dbContext.SaveChangesAsync();

            await _cache.SetStringAsync("ProductsCache", JsonSerializer.Serialize(_dbContext.Product));
            return Ok($"Updated {get.Id}");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Add([FromRoute] int id)
        {
            var product = await _dbContext.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound("404");
            }
            
            _dbContext.Product.Remove(product);
            await _dbContext.SaveChangesAsync();

            await _cache.SetStringAsync("ProductsCache", JsonSerializer.Serialize(_dbContext.Product));
            return Ok("Deleted");
        }
    }
}
