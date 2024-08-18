using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_commerce.Models;
using E_commerce.DTO;
using Microsoft.AspNetCore.Authorization;

namespace E_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ProductsController : ControllerBase
    {
        private readonly MyDb _context;

        public ProductsController(MyDb context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task <ActionResult<IEnumerable<Productview>>> GetAllProduct()
        {
            if (_context.Products == null)
            {
                return NotFound();
            }

            var products = await _context.Products.Include(c=>c.Category).Select
                (p=>new Productview
                {
                    Id=p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    Stock=p.Stock,
                    CategoryName = p.Category != null ? p.Category.Name : null

                }).ToListAsync();
            return Ok(products);
        }


        // GET: api/Products/5
        [HttpGet("id",Name = "GetProduct")]
        public async Task<ActionResult<Productview>> GetProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            
            var product = await _context.Products.Include(c=>c.Category).FirstOrDefaultAsync
                (x=>x.ProductId==id);

            if (product == null)
            {
                return NotFound();
            }

            Productview productview = new Productview();
            productview.Id = id;
            productview.Name = product.Name;
            productview.Description = product.Description;
            productview.Price = product.Price;
            productview.ImageUrl = product.ImageUrl;
            productview.Stock = product.Stock;
            productview.CategoryName = product.Category != null ? product.Category.Name : null;

            return Ok(productview);
        }


        // PUT: api/Products/5
        [HttpPut("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> PutProduct([FromRoute] int id,[FromBody] Productview productview)
        {
            if (id <=0 || productview==null)
            {
                return BadRequest();
            }
            var product = await _context.Products.Include(p=>p.Category).FirstOrDefaultAsync
                (x=>x.ProductId==id);
            if (product==null)
            {
                return NotFound();
            }

            // Update the product entity with the values from the DTO
            product.Name = productview.Name;
            product.Description = productview.Description;
            product.Price = productview.Price;
            product.Stock = productview.Stock;
            product.ImageUrl = productview.ImageUrl;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        //POST: api/Products
       [HttpPost]
       [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Productview>> PostProduct(Productview productview)
        {
            if (_context.Products == null)
            {
                return Problem("try another time");
            }
            // select may best , reduce memory as only fetch required fields.
            var category =await _context.Categories.FirstOrDefaultAsync(c=>c.Name==productview.CategoryName);
            if (category==null)
            {
                return BadRequest("Invalid category Name"); 
            }
            var product = new Product
            {
                Name = productview.Name,
                Description = productview.Description,
                Price = productview.Price,
                Stock = productview.Stock,
                ImageUrl = productview.ImageUrl,
                CategoryId = category.CategoryId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            // Map the product to Productview DTO
            Productview createdProductView = new Productview
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                CategoryName = category.Name
            };
            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId },createdProductView);
        }

        //DELETE: api/Products/5
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound("try in another time");
            }
            var product = await _context.Products.Include(p => p.OrderDetails)
                                         .Include(p => p.CartItems)
                                         .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound("invalid id");
            }

            // Option 1: Prevent deletion if there are related entities
            //if (product.OrderDetails.Any() || product.CartItems.Any())
            //{
            //    return BadRequest("Cannot delete product because it has related orders or cart items.");
            //}

            // Option 2: Manually delete related entities
            try
            {
                _context.OrderDetails.RemoveRange(product.OrderDetails);
                _context.CartItems.RemoveRange(product.CartItems);

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ) 
            {
                throw;
            }

            return Ok("success deleted");
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
