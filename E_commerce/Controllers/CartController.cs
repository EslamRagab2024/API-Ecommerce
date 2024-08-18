using E_commerce.DTO;
using E_commerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly MyDb _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CartController(MyDb context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cartview>>> GetAll()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null|| _context.Carts==null) { return NotFound(); }
            var carts=await _context.Carts.Include(c=>c.CartItems).FirstOrDefaultAsync(x=>x.UserId==userId);
            if (carts == null) {  return NotFound(); }
            var cartdto = new Cartview
            {
                Id = carts.CartId,
                CartItems = carts.CartItems.Select(ci => new CartItemview
                {
                    Id = ci.CartItemId,
                    Quantity = ci.Quantity,
                    ProductId = ci.ProductId
                }).ToList()
            };
            return Ok(cartdto);
        }

        [HttpPost("Items")]
        public async Task <ActionResult> Creat(CartItemCreateDto createDto) 
        {
            var userId = _userManager.GetUserId(User);
            if(userId==null|| _context.Carts==null) return NotFound();

            var cart= await _context.Carts.FirstOrDefaultAsync(x=>x.UserId==userId);  
            if (cart == null) 
            {
                cart= new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }
            var cartItem = new CartItem
            {
                CartId = cart.CartId,
                ProductId = createDto.ProductId,
                Quantity = createDto.Quantity
            };
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
            return Ok("seccess created");
        }

        [HttpPut("Items/{id}")]
        public async Task< ActionResult> Update(int id ,CartItemview cartItemview)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CartId == cartItem.CartId);

            if (cart == null || cart.UserId != userId)
            {
                return Forbid();
            }

            cartItem.Quantity = cartItemview.Quantity;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpDelete("Items/{id}")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CartId == cartItem.CartId);

            if (cart == null || cart.UserId != userId)
            {
                return Forbid();
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
