using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using frutility_backend.Data;
using frutility_backend.Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace frutility_backend.Controllers
{
    [Route("api/productreviews")]
    [ApiController]
    public class ProductReviewsController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductReviewsController(DataContext context)
        {
            _context = context;
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
        
        //POST: api/productreviews
        [HttpPost]
        public async Task<ActionResult<ProductReviews>> PostProductReviews(ProductReviews reviews)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Data");
            _context.ProductReviews.Add(reviews);
            await _context.SaveChangesAsync();
            return Ok();
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
