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
    [Route("api/subcategory")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        private readonly DataContext _context;

        public SubCategoryController(DataContext context)
        {
            _context = context;
        }

        //Get: api/subcategory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubCategory>>> GetSubcategory()
        {
            return await _context.SubCategories.ToListAsync();
        }

        //Get: api/subcategory/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SubCategory>> GetSubcategoryItem(int id)
        {
            SubCategory subCategory = await _context.SubCategories.FindAsync(id);
            if(subCategory == null)
            {
                return BadRequest();
            }
            return subCategory;
        }

        //POST: api/subcategory
        [HttpPost]
        public async Task<ActionResult<SubCategory>> PostSubcategory(SubCategory subCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }
            _context.SubCategories.Add(subCategory);
            await _context.SaveChangesAsync();
            return Ok();
        }

        //PUT: api/subcategory/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<SubCategory>> UpdateSubcategory(int id, SubCategory subCategoryrec)
        {
            if(id != subCategoryrec.SubCategoryID)
            {
                return BadRequest("Invalid Data");
            }
            _context.Entry(subCategoryrec).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        //Delete: api/subcategory/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<SubCategory>> DeleteSubcategory(int id)
        {
            SubCategory subCategory = await _context.SubCategories.FindAsync(id);
            if(subCategory == null)
            {
                return BadRequest();
            }
            _context.SubCategories.Remove(subCategory);
            await _context.SaveChangesAsync();
            return subCategory;
        }
    }
}
