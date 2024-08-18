using E_commerce.DTO;
using E_commerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly MyDb _context ;
        public CategoryController(MyDb context)
        {
            _context=context;
        }
        [HttpGet]
        public async Task< ActionResult<IEnumerable<Categoryview>>> GetAll()
        {
            if(_context.Categories==null)
            {
                return NotFound("try in another time");
            }
            var categories = await _context.Categories.Select(c => new Categoryview
            {
                Id=c.CategoryId,
                Name = c.Name,
                Products = c.Products.Select(p => new Productview
                {
                    Id=p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,Stock = p.Stock,ImageUrl = p.ImageUrl,CategoryName=p.Name
                }).ToList(),
            }).ToListAsync();

            return Ok(categories);

        }
        [HttpGet("Name")]
        public async Task<ActionResult<Categoryview>> GetByName(string Name)
        {
            if (_context.Categories == null)
            {
                return NotFound("try in another time");
            }
            var category = await _context.Categories.Include(p => p.Products)
                .FirstOrDefaultAsync(c => c.Name == Name);
            if (category == null) { return NotFound(); }
            var categoryview = new Categoryview
            {
                Id=category.CategoryId,Name = category.Name,
                Products = category.Products.Select(p => new Productview
                {Id=p.ProductId,Name = p.Name, Description = p.Description, Price=p.Price, ImageUrl=p.ImageUrl }).ToList()
            };
            return Ok(categoryview);
        }

        [HttpPut]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> Update(int id, Categoryview categoryview)
        {
            if (_context.Categories == null) { return NotFound(); }
            var category = await _context.Categories.Include(p => p.Products).FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null) { return NotFound(); }
            category.Name = categoryview.Name;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok("success updated");
            
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<Categoryview>> Create(Categoryview categoryview)
        {
            if (_context.Categories == null) { return NotFound(); }
            Category newcategory=new Category();    
            newcategory.Name = categoryview.Name;
            newcategory.Products=new List<Product>();
            _context.Categories.Add(newcategory);
            await _context.SaveChangesAsync();
            return Ok("Success created");
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task< ActionResult> Delete(int id) 
        {
           
            var category = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }

            if (category.Products.Any())
            {
                // Option 1: Prevent deletion if there are related products
                return BadRequest("Cannot delete category because there are related products.");

                // Option 2: Delete all related products before deleting the category
                // _context.Products.RemoveRange(category.Products);

                // Option 3: Reassign related products to a default category (e.g., Uncategorised)
                // var defaultCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Uncategorized");
                // if (defaultCategory == null)
                // {
                //     defaultCategory = new Category { Name = "Uncategorized" };
                //     _context.Categories.Add(defaultCategory);
                //     await _context.SaveChangesAsync();
                // }
                // foreach (var product in category.Products)
                // {
                //     product.CategoryId = defaultCategory.CategoryId;
                // }
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool ProductExists(int id)
        {
            return (_context.Categories?.Any(c=>c.CategoryId == id)).GetValueOrDefault();
        }
    }
}
