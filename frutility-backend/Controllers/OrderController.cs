using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using frutility_backend.Data;
using frutility_backend.Data.Model;
using frutility_backend.Data.ViewModel;
using frutility_backend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _hostingEnvironment;
        public OrderController(DataContext context, UserManager<ApplicationUser> userManager,
            IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }

        //Get: api/orders
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        //Get DateTime Now Order Count
        [Authorize]
        [Route("todayorderscount")]
        [HttpGet]
        public ActionResult<int> GetTodayCount()
        {
            int order = _context.Orders.Where(o => o.OrderDate.Date ==
            DateTime.Now.Date && o.OrderStatus == "PENDING").Count();
            return order;
        }

        //Get Pending Orders Count Other Than DateTime Now
        [Authorize]
        [Route("pendingorderscount")]
        [HttpGet]
        public ActionResult<int> GetPendingCount()
        {
            int order = _context.Orders.Where(o => o.OrderDate.Date !=
            DateTime.Now.Date && (o.OrderStatus == "DISPATCHED" || o.OrderStatus == "PENDING")).Count();
            return order;
        }

        //Get Delivered Orders Count
        [Authorize]
        [Route("deliveredorderscount")]
        [HttpGet]
        public ActionResult<int> GetDeliveredCount()
        {
            int order = _context.Orders.Where(o => o.OrderStatus == "DELIVERED").Count();
            return order;
        }

        //Get DateTime Now Orders List
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
                                   where (s.OrderDate.Date == DateTime.Now.Date &&
                                   s.OrderStatus == "PENDING")
                                   select new OrdersDetailsVM
                                   {
                                       Id = s.Id,
                                       Name = s.ApplicationUser.UserName,
                                       Email = s.ApplicationUser.Email,
                                       Phone = s.ApplicationUser.PhoneNumber,
                                       Address = s.ApplicationUser.ShippingAddress + ", "
                                       + s.ApplicationUser.ShippingCity + ", " +
                                       s.ApplicationUser.ShippingState,
                                       Product = s.Products.Name,
                                       Quantity = s.Quantity,
                                       Amount = s.Products.Price * s.Quantity,
                                       OrderDate = s.OrderDate,
                                       PaymentMethod = s.PaymentMethod,
                                       OrderStatus = s.OrderStatus,
                                       Remarks = s.Remarks
                                   }).ToListAsync();
                return Ok(order);
            }
            return Ok(decoded);
        }

        //Get Pending Orders List
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
                                   where (s.OrderDate.Date != DateTime.Now.Date &&
                                   (s.OrderStatus == "PENDING" || s.OrderStatus == "DISPATCHED"))
                                   select new OrdersDetailsVM
                                   {
                                       Id = s.Id,
                                       Name = s.ApplicationUser.UserName,
                                       Email = s.ApplicationUser.Email,
                                       Phone = s.ApplicationUser.PhoneNumber,
                                       Address = s.ApplicationUser.ShippingAddress + ", "
                                       + s.ApplicationUser.ShippingCity + ", " +
                                       s.ApplicationUser.ShippingState,
                                       Product = s.Products.Name,
                                       Quantity = s.Quantity,
                                       Amount = s.Products.Price * s.Quantity,
                                       OrderDate = s.OrderDate,
                                       PaymentMethod = s.PaymentMethod,
                                       OrderStatus = s.OrderStatus,
                                       Remarks = s.Remarks
                                   }).ToListAsync();
                return Ok(order);
            }
            return Ok(decoded);
        }

        //Get Delivered Orders List
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
                                   where (s.OrderStatus == "DELIVERED")
                                   select new OrdersDetailsVM
                                   {
                                       Id = s.Id,
                                       Name = s.ApplicationUser.UserName,
                                       Email = s.ApplicationUser.Email,
                                       Phone = s.ApplicationUser.PhoneNumber,
                                       Address = s.ApplicationUser.ShippingAddress + ", "
                                       + s.ApplicationUser.ShippingCity + ", " +
                                       s.ApplicationUser.ShippingState,
                                       Product = s.Products.Name,
                                       Quantity = s.Quantity,
                                       Amount = s.Products.Price * s.Quantity,
                                       OrderDate = s.OrderDate,
                                       PaymentMethod = s.PaymentMethod,
                                       Remarks = s.Remarks,
                                       OrderStatus = s.OrderStatus
                                   }).ToListAsync();
                return Ok(order);
            }
            return Ok(decoded);
        }
        //Change Order Status Flag
        [Authorize]
        [Route("orderstatus")]
        [HttpPut]
        public async Task<ActionResult> UpdateOrderStatus(OrderStatusVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }
            Order order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == model.Id);
            order.OrderStatus = model.OrderStatus;
            order.Remarks = model.Remarks;
            await _context.SaveChangesAsync();
            return Ok(true);
        }

        //Get: api/orders/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUserID(string id)
        {
            var orders = await _context.Orders.Where(u => u.UserId == id).ToListAsync();
            return orders;
        }

        //Get: api/orders/userordercount
        [Authorize]
        [Route("userordercount")]
        [HttpGet]
        public async Task<ActionResult> GetUserOrderCount()
        {
            if (Request.Headers.ContainsKey("Authorization"))
            {
                string[] token = Request.Headers.GetCommaSeparatedValues("Authorization");
                string strtoken = String.Concat(token);
                strtoken = strtoken.Replace("Bearer ", "");
                var result = gettoken(strtoken);
                var order = await _context.Orders.Where(u => u.UserId == result.Result 
                && u.OrderStatus == "NOTCONFIRMED").CountAsync();
                return Ok(order);
            }
            return Ok(false);
        }

        //Get: api/orders/shoppingcart
        [Authorize]
        [Route("shoppingcart")]
        [HttpGet]
        public async Task<ActionResult> GetShoppingCartItems()
        {
            if (Request.Headers.ContainsKey("Authorization"))
            {
                string[] token = Request.Headers.GetCommaSeparatedValues("Authorization");
                string strtoken = String.Concat(token);
                strtoken = strtoken.Replace("Bearer ", "");
                var result = gettoken(strtoken);
                var orderdetail = await _context.Orders
                    .Include(p => p.Products).Where(u=>u.UserId == result.Result && 
                    u.OrderStatus == "NOTCONFIRMED").ToListAsync();
                List<OrderCartVM> orders = new List<OrderCartVM>();
                foreach(var order in orderdetail)
                {
                    List<byte[]> imageBytes = new List<byte[]>();
                    if (order.Products.Image1 != null)
                    {
                        string path = "Assets/images/" + order.Products.Image1;
                        string filepath = Path.Combine(_hostingEnvironment.ContentRootPath, path);
                        byte[] bytes = await System.IO.File.ReadAllBytesAsync(filepath);
                        imageBytes.Add(bytes);
                        orders.Add(new OrderCartVM
                        {
                            Id = order.Id,
                            UserId = order.UserId,
                            ProductId = order.ProductId,
                            Quantity = order.Quantity,
                            OrderDate = order.OrderDate,
                            PaymentMethod = order.PaymentMethod,
                            OrderStatus = order.OrderStatus,
                            ApplicationUser = order.ApplicationUser,
                            Products = order.Products,
                            ImageBytes = imageBytes
                        });
                    }
                }
                return Ok(orders);
            }
            return Ok(false);
        }

        //POST: api/orders
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> PostOrder(OrderPostVM orderPost)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }
            if (Request.Headers.ContainsKey("Authorization"))
            {
                string[] token = Request.Headers.GetCommaSeparatedValues("Authorization");
                string strtoken = String.Concat(token);
                strtoken = strtoken.Replace("Bearer ", "");
                var result = gettoken(strtoken);
                string userId = result.Result;
                Order order = new Order
                {
                    UserId = Convert.ToString(userId),
                    ProductId = orderPost.ProductId,
                    Quantity = orderPost.Quantity,
                    OrderDate = DateTime.Now,
                    PaymentMethod = "COD",
                    OrderStatus = "NOTCONFIRMED"
                };
                _context.Add(order);
                await _context.SaveChangesAsync();
                return Ok(order);
            }
            return Ok(false);
        }

        //Get userroles through recieved token
        public async Task<string> gettoken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokens = handler.ReadToken(token) as JwtSecurityToken;
            var decoded = tokens.Claims.FirstOrDefault(t => t.Type == "unique_name");
            var user = await _userManager.FindByIdAsync(decoded.Value);
            return user.Id;
        }
        //PUT: api/orders/
        [Authorize]
        [HttpPut]
        public async Task<ActionResult<Order>> UpdateOrder()
        {
            if (Request.Headers.ContainsKey("Authorization"))
            {
                string[] token = Request.Headers.GetCommaSeparatedValues("Authorization");
                string strtoken = String.Concat(token);
                strtoken = strtoken.Replace("Bearer ", "");
                var result = gettoken(strtoken);
                string userId = result.Result;
                _context.Orders.Where(u => u.UserId == userId).ToList().ForEach(o => o.OrderStatus = "PENDING");
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            return NoContent();
        }

        //Delete: api/orders/{id}
        [Authorize]
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
            return Ok(true);
        }
    }
}
