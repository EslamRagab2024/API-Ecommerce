using E_commerce.DTO;
using E_commerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks.Dataflow;

namespace E_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly MyDb _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public OrderController(MyDb context , UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Orderview>>> GetAll()
        {
            
            // check user authentcated
            var userid = _userManager.GetUserId(User);
            if (userid == null|| _context.Orders == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders.Where(o => o.UserId == userid)
                .Include(d => d.OrderDetails).ToListAsync();
            
            //  DTO MODEL
            var orderdto = orders.Select(o => new Orderview
            {
                Id = o.OrderId,OrderDate = o.OrderDate,TotalAmount = o.TotalAmount,
                OrderItems = o.OrderDetails.Select(od => new OrderDetailview
                {
                    ProductId = od.ProductId,Price = od.Price,Quantity = od.Quantity
                }).ToList()
            }).ToList();

            return Ok(orderdto);
        }

        [HttpGet("id")]
        public async Task<ActionResult<Orderview>> Get(int id)
        {
            var userid = _userManager.GetUserId(User);
            if (userid == null || _context.Orders == null) { return NotFound(); }
            var orders = await _context.Orders.Include(od=>od.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id&& o.UserId==userid);
            if (orders == null)
            {
                return NotFound();
            }
            var result = new Orderview
            {
                Id = orders.OrderId,OrderDate = orders.OrderDate,TotalAmount = orders.TotalAmount,
                OrderItems = orders.OrderDetails.Select(oi => new OrderDetailview
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task <ActionResult> Create(Ordercreate ordercreate)
        {
            var userid= _userManager.GetUserId(User);
            var order = new Order
            {
                UserId = userid,
                OrderDate = DateTime.Now,
                TotalAmount = ordercreate.TotalAmount,
                OrderDetails = ordercreate.OrderItems.Select(oi => new OrderDetail
                {
                    ProductId = oi.ProductId, Price = oi.Price, Quantity = oi.Quantity
                }).ToList()
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Ok("success add");
        }
        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
