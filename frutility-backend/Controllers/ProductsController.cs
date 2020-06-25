using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using frutility_backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using frutility_backend.Data.Model;

namespace frutility_backend.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Products>>> GetProducts()
        {
            var productlist = await _context.Products.ToListAsync();
            return Ok(productlist);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Products>> GetProductItem(int id)
        {
            var productitem = await _context.Products.FindAsync(id);
            return productitem;
        }
        [HttpPost]
        public async Task<ActionResult<Products>> PostProducts(Products productsrec)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }
            _context.Products.Add(productsrec);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
