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
using System.Linq.Expressions;

namespace frutility_backend.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly string route = "https://192.168.2.127:5001/api/products/";

        public ProductsController(DataContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        //Get All Products
        //api/products
        [HttpGet]
        public async Task<ActionResult<ProductsGetVM>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            List<ProductsGetVM> productget = new List<ProductsGetVM>();
            foreach (var product in products)
            {
                if (product.Image1 != null)
                {
                    
                    productget.Add(new ProductsGetVM
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Vendor = product.Vendor,
                        Price = product.Price,
                        PriceBeforeDiscount = product.PriceBeforeDiscount,
                        imageURI = route + product.Image1,
                        ShippingCharges = product.ShippingCharges,
                        Availability = product.Availability,
                        Stock = product.Stock,
                        PostingDate = product.PostingDate,
                        UpdationDate = product.UpdationDate,
                        PackageWeight = product.PackageWeight,
                        SubCategoryID = product.SubCategoryID
                    });
                }
            }
            return Ok(productget);
        }
        [HttpGet("{filename}")]
        public async Task<IActionResult> GetImages(string filename)
        {
            string path = "Assets/images/" + filename;
            string filepath = Path.Combine(_hostingEnvironment.ContentRootPath, path);
            byte[] b = await System.IO.File.ReadAllBytesAsync(filepath);
            return File(b, "image/jpeg");
        }

        //Get single product with single image
        [HttpGet("productbyid/{id}")]
        public async Task<ActionResult<ProductsGetVM>> GetProductMinById(int id)
        {
            Products model = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (model.Image1 != null)
            {
                ProductsGetVM productsGet = new ProductsGetVM
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    Vendor = model.Vendor,
                    Price = model.Price,
                    PriceBeforeDiscount = model.PriceBeforeDiscount,
                    imageURI = model.Image1,
                    ShippingCharges = model.ShippingCharges,
                    Availability = model.Availability,
                    Stock = model.Stock,
                    PostingDate = model.PostingDate,
                    UpdationDate = model.UpdationDate,
                    PackageWeight = model.PackageWeight,
                    SubCategoryID = model.SubCategoryID
                };
                return productsGet;
            }
            return NoContent();
        }



        //Post: api/products
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Products>> PostProducts([FromForm] ProductsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }
            string UniqueFileNameimage1 = null;
            if (model.Image1 != null)
            {
                string uploadfolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets/images");
                UniqueFileNameimage1 = Guid.NewGuid().ToString() + "_" + model.Image1.FileName;
                string filepath = Path.Combine(uploadfolder, UniqueFileNameimage1);
                using (var stream = System.IO.File.Create(filepath))
                {
                    await model.Image1.CopyToAsync(stream);
                }
            }
            Products products = new Products
            {
                Name = model.Name,
                Description = model.Description,
                Vendor = model.Vendor,
                Price = model.Price,
                PriceBeforeDiscount = model.PriceBeforeDiscount,
                Image1 = UniqueFileNameimage1,
                ShippingCharges = model.ShippingCharges,
                Availability = model.Availability,
                Stock = model.Stock,
                PostingDate = DateTime.Now,
                PackageWeight = model.PackageWeight,
                SubCategoryID = model.SubCategoryID
            };
            _context.Products.Add(products);
            await _context.SaveChangesAsync();
            return Ok(products);
        }

        //Post: api/products
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<Products>> UpdateProduct(long id,[FromForm]ProductUpdateVM model)
        {
            if (id != model.Id)
                return BadRequest("Invalid Data");
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == model.Id);
            string UniqueFileNameimage1 = null;
            if (model.ImageNo1)
            {
                if (model.Image1 != null)
                {
                    string uploadfolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets/images");
                    UniqueFileNameimage1 = Guid.NewGuid().ToString() + "_" + model.Image1.FileName;
                    string filepath = Path.Combine(uploadfolder, UniqueFileNameimage1);
                    if (product.Image1 != null)
                    {
                        string prevFile = Path.Combine(uploadfolder, product.Image1);
                        System.IO.File.Delete(prevFile);
                    }
                    using (var stream = System.IO.File.Create(filepath))
                    {
                        await model.Image1.CopyToAsync(stream);
                    }
                }
            }
            product.Name = model.Name;
            product.Description = model.Description;
            product.Vendor = model.Vendor;
            product.Price = model.Price;
            product.PriceBeforeDiscount = model.PriceBeforeDiscount;
            product.Image1 = (UniqueFileNameimage1 != null) ? UniqueFileNameimage1 : product.Image1;
            product.ShippingCharges = model.ShippingCharges;
            product.Availability = model.Availability;
            product.Stock = model.Stock;
            product.UpdationDate = DateTime.Now;
            product.PackageWeight = model.PackageWeight;
            product.SubCategoryID = model.SubCategoryID;
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(product);
        }


        //Delete: api/products/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Products>> DeleteProducts(int id)
        {
            Products products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return BadRequest();
            }
            if (products.Image1 != null)
            {
                string uploadfolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets/images");
                string filepath = Path.Combine(uploadfolder, products.Image1);
                System.IO.File.Delete(filepath);
            }
            _context.Products.Remove(products);
            await _context.SaveChangesAsync();
            return products;
        }
    }
}
