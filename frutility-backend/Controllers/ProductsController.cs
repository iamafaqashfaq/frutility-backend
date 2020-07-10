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
using frutility_backend.Data.ViewModel;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using SQLitePCL;

namespace frutility_backend.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ProductsController(DataContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
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
        //async Task<ActionResult<Products>>
        //Get: api/products/{id}
        //[HttpGet("image/{id}")]
        //public async Task<IActionResult> GetProductItem(int id)
        //{
        //    Products model = _context.Products.FirstOrDefault(p => p.Id == id);
        //    if(model.Image1 != null)
        //    {
        //        string path = "Assests/images/" + model.Image1;
        //        string filepath = Path.Combine(_hostingEnvironment.ContentRootPath, path);
        //        var memory = new MemoryStream();
        //        using (var stream = new FileStream(filepath, FileMode.Open))
        //        {
        //            await stream.CopyToAsync(memory);
        //        }
        //        if(model.Image2 != null)
        //        {
        //            path = "Assests/images/" + model.Image2;
        //            filepath = Path.Combine(_hostingEnvironment.ContentRootPath, path);
        //            using (var stream = new FileStream(filepath, FileMode.Open))
        //            {
        //                await stream.CopyToAsync(memory);
        //            }
        //        }
        //        if (model.Image3 != null)
        //        {
        //            path = "Assests/images/" + model.Image3;
        //            filepath = Path.Combine(_hostingEnvironment.ContentRootPath, path);
        //            using (var stream = new FileStream(filepath, FileMode.Open))
        //            {
        //                await stream.CopyToAsync(memory);
        //            }
        //        }
        //        memory.Position = 1;
        //        return File(memory, "image/jpeg");
        //    }
        //    return NoContent();
        //}

        [HttpGet("image/{id}")]
        public async Task<List<byte[]>> GetProductItem(int id)
        {
            Products model = _context.Products.FirstOrDefault(p => p.Id == id);
            List<byte[]> imageBytes = new List<byte[]>();
            if (model.Image1 != null)
            {
                string path = "Assests/images/" + model.Image1;
                string filepath = Path.Combine(_hostingEnvironment.ContentRootPath, path);
                var memory = new MemoryStream();
                byte[] bytes = await System.IO.File.ReadAllBytesAsync(filepath);
                imageBytes.Add(bytes);
                if (model.Image2 != null)
                {
                    path = "Assests/images/" + model.Image2;
                    filepath = Path.Combine(_hostingEnvironment.ContentRootPath, path);
                    bytes = await System.IO.File.ReadAllBytesAsync(filepath);
                    imageBytes.Add(bytes);
                }
                if (model.Image3 != null)
                {
                    path = "Assests/images/" + model.Image3;
                    filepath = Path.Combine(_hostingEnvironment.ContentRootPath, path);
                    bytes = await System.IO.File.ReadAllBytesAsync(filepath);
                    imageBytes.Add(bytes);
                }
                return imageBytes;
            }
            return imageBytes;
        }



        //Post: api/products
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Products>> PostProducts([FromForm]ProductsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }
            string UniqueFileNameimage1 = null;
            if (model.Image1 != null)
            {
                string uploadfolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Assests/images");
                UniqueFileNameimage1 = Guid.NewGuid().ToString() + "_" + model.Image1.FileName;
                string filepath = Path.Combine(uploadfolder, UniqueFileNameimage1);
                await model.Image1.CopyToAsync(new FileStream(filepath, FileMode.Create));
            }
            string UniqueFileNameimage2 = null;
            if (model.Image2 != null)
            {
                string uploadfolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Assests/images");
                UniqueFileNameimage2 = Guid.NewGuid().ToString() + "_" + model.Image2.FileName;
                string filepath = Path.Combine(uploadfolder, UniqueFileNameimage2);
                await model.Image2.CopyToAsync(new FileStream(filepath, FileMode.Create));
            }
            string UniqueFileNameimage3 = null;
            if (model.Image3 != null)
            {
                string uploadfolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Assests/images");
                UniqueFileNameimage3 = Guid.NewGuid().ToString() + "_" + model.Image3.FileName;
                string filepath = Path.Combine(uploadfolder, UniqueFileNameimage3);
                await model.Image3.CopyToAsync(new FileStream(filepath, FileMode.Create));
            }
            Products products = new Products
            {
                Name = model.Name,
                Description = model.Description,
                Vendor = model.Vendor,
                Price = model.Price,
                PriceBeforeDiscount = model.PriceBeforeDiscount,
                Image1 = UniqueFileNameimage1,
                Image2 = UniqueFileNameimage2,
                Image3 = UniqueFileNameimage3,
                ShippingCharges = model.ShippingCharges,
                Availability = model.Availability,
                Stock = model.Stock,
                PostingDate = DateTime.Now,
                UpdationDate = DateTime.Now,
                PackageWeight = model.PackageWeight,
                SubCategoryID = model.SubCategoryID
            };
            _context.Products.Add(products);
            await _context.SaveChangesAsync();
            return Ok(products);
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
