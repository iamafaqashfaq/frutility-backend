using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using frutility_backend.Data;
using frutility_backend.Data.Model;
using frutility_backend.Data.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace frutility_backend.Controllers
{
    [Route("api/productreviews")]
    [ApiController]
    public class ProductReviewsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductReviewsController(DataContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Get: api/productreviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductReviews>>> GetProductReview()
        {
            return await _context.ProductReviews.ToListAsync();
        }

        //Get: api/productreviews/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ProductReviews>>> GetProductReviewById(int id)
        {
            var productReviews = 
                await _context.ProductReviews.Where(i => i.ProductId == id).ToListAsync();
            return productReviews;
        }

        //Get: api/productreviews/starrating/{id}
        [Route("starrating/{id}")]
        [HttpGet("starrating/{id}")]
        public async Task<ActionResult> GetStarRatingById(int id)
        {
            var star = await _context.ProductReviews.Where(o => o.Quality == 1).CountAsync();
            var star1 = await _context.ProductReviews.Where(o => o.Quality == 2).CountAsync();
            var star2 = await _context.ProductReviews.Where(o => o.Quality == 3).CountAsync();
            var star3 = await _context.ProductReviews.Where(o => o.Quality == 4).CountAsync();
            var star4 = await _context.ProductReviews.Where(o => o.Quality == 5).CountAsync();
            GetStarRatingVM rating = new GetStarRatingVM
            {
                One = star,
                Two = star1,
                Three = star2,
                Four = star3,
                Five = star4
            };
            return Ok(rating);
        }

        //POST: api/productreviews
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ProductReviews>> PostProductReviews(PostReviewVM reviews)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Data");
            var user = await _userManager.FindByIdAsync(User.Identity.Name);
            ProductReviews pr = new ProductReviews
            {
                ProductId = reviews.ProductId,
                Quality = reviews.Quality,
                Review = reviews.Review,
                Name = user.UserName,
                ReviewDate = DateTime.Now
            };
            await _context.ProductReviews.AddAsync(pr);
            await _context.SaveChangesAsync();
            return Ok(true);
        }

        //PUT: api/productreviews/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductReviews>> UpdateProductReviews(int id, ProductReviews review)
        {
            if (id != review.Id)
                return BadRequest("Invalid Data");
            _context.Entry(review).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        //Delete: api/productreviews/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductReviews>> DeleteProductReviews(int id)
        {
            var review = await _context.ProductReviews.FindAsync(id);
            if(review == null)
            {
                return NotFound();
            }
            _context.ProductReviews.Remove(review);
            await _context.SaveChangesAsync();
            return review;
        }
    }
}
