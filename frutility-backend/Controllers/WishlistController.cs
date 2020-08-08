using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using frutility_backend.Data;
using frutility_backend.Data.Model;
using frutility_backend.Data.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace frutility_backend.Controllers
{
    [Route("api/wishlist")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public WishlistController(DataContext context, UserManager<ApplicationUser> userManager, 
            IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetWishlist()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByIdAsync(User.Identity.Name);
                var wishlist = await _context.Wishlists.Include(p => p.Products).
                    Where(w => w.UserId == user.Id).ToListAsync();
                List<WishlistGetVM> WishlistGet = new List<WishlistGetVM>();

                foreach (var list in wishlist)
                {
                    List<byte[]> imageBytes = new List<byte[]>();
                    if (list.Products.Image1 != null)
                    {
                        string path = "Assets/images/" + list.Products.Image1;
                        string filepath = Path.Combine(_hostingEnvironment.ContentRootPath, path);
                        byte[] bytes = await System.IO.File.ReadAllBytesAsync(filepath);
                        imageBytes.Add(bytes);
                        WishlistGet.Add(new WishlistGetVM
                        {
                            Id = list.Id,
                            productId = list.ProductId,
                            ProductName = list.Products.Name,
                            ProductPrice = list.Products.Price,
                            ImageBytes = imageBytes
                        });
                        return Ok(WishlistGet);
                    }
                }
            }
            return Ok(false);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> PostWishlist(WishlistVM model)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByIdAsync(User.Identity.Name);
                if(!(await _context.Wishlists.AnyAsync(w => w.ProductId == model.ProductId
                && w.UserId == user.Id)))
                {
                    Wishlist wishlist = new Wishlist
                    {
                        UserId = user.Id,
                        ProductId = model.ProductId,
                        PostingDate = DateTime.Now
                    };
                    await _context.AddAsync(wishlist);
                    await _context.SaveChangesAsync();
                    return Ok(true);
                }
            }
            return Ok(false);
        }
        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> DeleteWishlist(WishlistVM model)
        {
            if (User.Identity.IsAuthenticated)
            {
                Wishlist wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.Id == model.Id);
                if(wishlist != null)
                {
                    _context.Wishlists.Remove(wishlist);
                    await _context.SaveChangesAsync();
                    return Ok(true);
                }
            }
            return Ok(false);
        }
    }
}
