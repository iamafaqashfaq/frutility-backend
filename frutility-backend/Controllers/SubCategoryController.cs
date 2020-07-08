using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using frutility_backend.Data;
using frutility_backend.Data.Model;
using Microsoft.AspNetCore.Authorization;
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
            var subcategory = await (from s in _context.SubCategories
                                     select new
                                     {
                                         s.ID,
                                         s.SubcategoryName,
                                         s.CategoryID,
                                         s.Category.CategoryName,
                                         s.CreationDate,
                                         s.UpdationDate
                                     }).ToListAsync();
            return Ok(subcategory);
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
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<SubCategory>> PostSubcategory(SubCategory subCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }
            subCategory.CreationDate = DateTime.Now;
            _context.SubCategories.Add(subCategory);
            await _context.SaveChangesAsync();
            return Ok(true);
        }

        //PUT: api/subcategory/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<SubCategory>> UpdateSubcategory(long id, SubCategory subCategoryrec)
        {
            if(id != subCategoryrec.ID)
            {
                return BadRequest("Invalid Data");
            }
            subCategoryrec.UpdationDate = DateTime.Now;
            _context.Entry(subCategoryrec).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(true);
        }

        //Delete: api/subcategory/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSubcategory(int id)
        {
            SubCategory subCategory = await _context.SubCategories.FindAsync(id);
            if(subCategory == null)
            {
                return BadRequest();
            }
            _context.SubCategories.Remove(subCategory);
            await _context.SaveChangesAsync();
            return Ok(true);
        }
    }
}
