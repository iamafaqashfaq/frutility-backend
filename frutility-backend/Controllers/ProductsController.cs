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

        // Get: /api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Products>>> GetProducts()
        {
            var productlist = await (from p in _context.Products
                                     select new
                                     {
                                         p.Id,
                                         p.Name,
                                         p.Description,
                                         p.Vendor,
                                         p.Price,
                                         p.PriceBeforeDiscount,
                                         p.Image1,
                                         p.Image2,
                                         p.Image3,
                                         p.ShippingCharges,
                                         p.Availability,
                                         p.Stock,
                                         p.PostingDate,
                                         p.UpdationDate,
                                         p.PackageWeight,
                                         p.SubCategoryID,
                                         p.SubCategory.SubcategoryName
                                     }).ToListAsync();
            return Ok(productlist);
        }

        //Get: api/products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Products>> GetProductItem(int id)
        {
            var productitem = await _context.Products.FindAsync(id);
            return productitem;
        }

        //Post: api/products
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

        //Put: api/products/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Products>> UpdateProducts(int id, Products productsrec)
        {
            if(id != productsrec.Id)
            {
                return BadRequest();
            }
            _context.Entry(productsrec).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        //Delete: api/products/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<Products>> DeleteProducts(int id)
        {
            Products products = await _context.Products.FindAsync(id);
            if(products == null)
            {
                return BadRequest();
            }
            _context.Products.Remove(products);
            await _context.SaveChangesAsync();
            return products;
        }
    }
}
