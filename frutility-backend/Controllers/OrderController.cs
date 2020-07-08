using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using frutility_backend.Data;
using frutility_backend.Data.Model;
using frutility_backend.Data.ViewModel;
using frutility_backend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace frutility_backend.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public OrderController(DataContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Get: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        [Authorize]
        [Route("todayorderscount")]
        [HttpGet]
        public ActionResult<int> GetTodayCount()
        {
            int order = _context.Orders.Where(o => o.OrderDate.Date ==
            DateTime.Now.Date && o.OrderStatus == "Pending").Count();
            return order;
        }

        [Authorize]
        [Route("pendingorderscount")]
        [HttpGet]
        public ActionResult<int> GetPendingCount()
        {
            int order = _context.Orders.Where(o => o.OrderDate.Date !=
            DateTime.Now.Date || o.OrderStatus == "Dispatched").Count();
            return order;
        }

        [Authorize]
        [Route("deliveredorderscount")]
        [HttpGet]
        public ActionResult<int> GetDeliveredCount()
        {
            int order = _context.Orders.Where(o => o.OrderStatus == "Delivered").Count();
            return order;
        }

        [Authorize]
        [Route("todayorders")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Order>>> GetTodayOrders(Token token)
        {
            DecodeToken dt = new DecodeToken();
            var decoded = dt.TokenDecoder(token.entoken);
            var user = await _userManager.FindByIdAsync(decoded.Value);
            var result = _userManager.GetRolesAsync(user);
            if (result.Result.Contains("Admin"))
            {
                var order = await (from s in _context.Orders
                                   where s.OrderDate.Date == DateTime.Now.Date
                                   select new OrdersDetailsVM
                                   {
                                       Id = s.Id,
                                       Name = s.ApplicationUser.UserName,
                                       Email = s.ApplicationUser.Email,
                                       Phone = s.ApplicationUser.PhoneNumber,
                                       Address = s.ApplicationUser.ShippingAddress,
                                       Product = s.Products.Name,
                                       Quantity = s.Quantity,
                                       Amount = s.Products.Price * s.Quantity,
                                       OrderDate = s.OrderDate,
                                       PaymentMethod = s.PaymentMethod,
                                       OrderStatus = s.OrderStatus
                                   }).ToListAsync();
                return Ok(order);
            }
            return Ok(decoded);
        }


        [Authorize]
        [Route("pendingorders")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Order>>> GetPendingOrders(Token token)
        {
            DecodeToken dt = new DecodeToken();
            var decoded = dt.TokenDecoder(token.entoken);
            var user = await _userManager.FindByIdAsync(decoded.Value);
            var result = _userManager.GetRolesAsync(user);
            if (result.Result.Contains("Admin"))
            {
                var order = await (from s in _context.Orders
                                   where (s.OrderDate.Date != DateTime.Now.Date && s.OrderStatus == "pending")
                                   select new OrdersDetailsVM
                                   {
                                       Id = s.Id,
                                       Name = s.ApplicationUser.UserName,
                                       Email = s.ApplicationUser.Email,
                                       Phone = s.ApplicationUser.PhoneNumber,
                                       Address = s.ApplicationUser.ShippingAddress,
                                       Product = s.Products.Name,
                                       Quantity = s.Quantity,
                                       Amount = s.Products.Price * s.Quantity,
                                       OrderDate = s.OrderDate,
                                       PaymentMethod = s.PaymentMethod,
                                       OrderStatus = s.OrderStatus
                                   }).ToListAsync();
                return Ok(order);
            }
            return Ok(decoded);
        }

        [Authorize]
        [Route("deliveredorders")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Order>>> DeliveredOrders(Token token)
        {
            DecodeToken dt = new DecodeToken();
            var decoded = dt.TokenDecoder(token.entoken);
            var user = await _userManager.FindByIdAsync(decoded.Value);
            var result = _userManager.GetRolesAsync(user);
            if (result.Result.Contains("Admin"))
            {
                var order = await (from s in _context.Orders
                                   where (s.OrderDate.Date != DateTime.Now.Date && s.OrderStatus == "Delivered")
                                   select new OrdersDetailsVM
                                   {
                                       Id = s.Id,
                                       Name = s.ApplicationUser.UserName,
                                       Email = s.ApplicationUser.Email,
                                       Phone = s.ApplicationUser.PhoneNumber,
                                       Address = s.ApplicationUser.ShippingAddress,
                                       Product = s.Products.Name,
                                       Quantity = s.Quantity,
                                       Amount = s.Products.Price * s.Quantity,
                                       OrderDate = s.OrderDate,
                                       PaymentMethod = s.PaymentMethod,
                                       OrderStatus = s.OrderStatus
                                   }).ToListAsync();
                return Ok(order);
            }
            return Ok(decoded);
        }
        //Get: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUserID(string id)
        {
            var orders = await _context.Orders.Where(u => u.UserId == id).ToListAsync();
            return orders;
        }

        //POST: api/orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Data");
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Ok();
        }

        //PUT: api/orders/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Order>> UpdateOrder(int id, Order order)
        {
            if (id != order.Id)
                return BadRequest("Invalid Data");
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        //Delete: api/delete/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<Order>> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if(order == null)
            {
                return NotFound();
            }
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return order;
        }
    }
}
