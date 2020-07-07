using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Threading.Tasks;
using frutility_backend.Data;
using frutility_backend.Data.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.DataClassification;
using Microsoft.EntityFrameworkCore;

namespace frutility_backend.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly DataContext _context;

        public CategoryController(DataContext context)
        {
            _context = context;
        }

        // Get: api/category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategory()
        {
            var categorylist = await _context.Categories.ToListAsync();
            return Ok(categorylist);
        }

        // Get: api/category/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategoryItem(int id)
        {
            var Categoryitem = await _context.Categories.FindAsync(id);
            return Categoryitem;
        }

        // Post: api/category
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category categoryrec)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }
            categoryrec.CreationDate = DateTime.Now;
            _context.Categories.Add(categoryrec);
            await _context.SaveChangesAsync();
            return Ok(true);
        }

        // Put: api/category/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> UpdateCategory(Category categoryrec)
        {
            var categories = await _context.Categories.FindAsync(categoryrec.Id);
            if(categories == null)
            {
                return BadRequest();
            }
            categories = null;
            categoryrec.UpdationDate = DateTime.Now;
            _context.Entry(categoryrec).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(true);
        }

        // Delete: /api/category/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> DeleteCategory(int id)
        {
            Category category = await _context.Categories.FindAsync(id);
            if(category == null)
            {
                return BadRequest();
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return category;
        }
    }
}
