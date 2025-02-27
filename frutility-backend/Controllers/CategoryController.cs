﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Threading.Tasks;
using frutility_backend.Data;
using frutility_backend.Data.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _hostingEnvironment;
        public CategoryController(DataContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
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
        public async Task<IActionResult> PostCategory(Category categoryrec)
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
        public async Task<IActionResult> UpdateCategory(long id, Category categoryrec)
        {
            if(id != categoryrec.Id)
            {
                return BadRequest();
            }
            var category = await _context.Categories.FindAsync(categoryrec.Id);
            category.CategoryName = categoryrec.CategoryName;
            category.Description = categoryrec.Description;
            category.UpdationDate = DateTime.Now;
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(true);
        }

        // Delete: /api/category/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            Category category = await _context.Categories.FindAsync(id);
            if(category == null)
            {
                return BadRequest();
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return Ok(true);
        }
    }
}
