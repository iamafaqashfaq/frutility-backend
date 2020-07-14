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
                List<byte[]> imageBytes = new List<byte[]>();
                if (product.Image1 != null)
                {
                    string path = "Assets/images/" + product.Image1;
                    string filepath = Path.Combine(_hostingEnvironment.ContentRootPath, path);
                    byte[] bytes = await System.IO.File.ReadAllBytesAsync(filepath);
                    imageBytes.Add(bytes);
                    if (product.Image2 != null)
                    {
                        path = "Assets/images/" + product.Image2;
                        filepath = Path.Combine(_hostingEnvironment.ContentRootPath, path);
                        bytes = await System.IO.File.ReadAllBytesAsync(filepath);
                        imageBytes.Add(bytes);
                    }
                    if (product.Image3 != null)
                    {
                        path = "Assets/images/" + product.Image3;
                        filepath = Path.Combine(_hostingEnvironment.ContentRootPath, path);
                        bytes = await System.IO.File.ReadAllBytesAsync(filepath);
                        imageBytes.Add(bytes);
                    }
                    productget.Add(new ProductsGetVM
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Vendor = product.Vendor,
                        Price = product.Price,
                        PriceBeforeDiscount = product.PriceBeforeDiscount,
                        ImageBytes = imageBytes,
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

        //Get Products with a single image
        [HttpGet("productmin")]
        public async Task<ActionResult<ProductsGetVM>> GetProductsMin()
        {
            var products = await _context.Products.ToListAsync();
            List<ProductsGetVM> productget = new List<ProductsGetVM>();
            foreach (var product in products)
            {
                List<byte[]> imageBytes = new List<byte[]>();
                if (product.Image1 != null)
                {
                    string path = "Assets/images/" + product.Image1;
                    string filepath = Path.Combine(_hostingEnvironment.ContentRootPath, path);
                    byte[] bytes = await System.IO.File.ReadAllBytesAsync(filepath);
                    imageBytes.Add(bytes);
                    productget.Add(new ProductsGetVM
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Vendor = product.Vendor,
                        Price = product.Price,
                        PriceBeforeDiscount = product.PriceBeforeDiscount,
                        ImageBytes = imageBytes,
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

        //Get single product with single image
        [HttpGet("productminbyid/{id}")]
        public async Task<ActionResult<ProductsGetVM>> GetProductMinById(int id)
        {
            Products model = _context.Products.FirstOrDefault(p => p.Id == id);
            List<byte[]> imageBytes = new List<byte[]>();
            if (model.Image1 != null)
            {
                string path = "Assets/images/" + model.Image1;
                string filepath = Path.Combine(_hostingEnvironment.ContentRootPath, path);
                byte[] bytes = await System.IO.File.ReadAllBytesAsync(filepath);
                imageBytes.Add(bytes);
                ProductsGetVM productsGet = new ProductsGetVM
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    Vendor = model.Vendor,
                    Price = model.Price,
                    PriceBeforeDiscount = model.PriceBeforeDiscount,
                    ImageBytes = imageBytes,
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
            string UniqueFileNameimage2 = null;
            if (model.Image2 != null)
            {
                string uploadfolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets/images");
                UniqueFileNameimage2 = Guid.NewGuid().ToString() + "_" + model.Image2.FileName;
                string filepath = Path.Combine(uploadfolder, UniqueFileNameimage2);
                using (var stream = System.IO.File.Create(filepath))
                {
                    await model.Image2.CopyToAsync(stream);
                }
            }
            string UniqueFileNameimage3 = null;
            if (model.Image3 != null)
            {
                string uploadfolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets/images");
                UniqueFileNameimage3 = Guid.NewGuid().ToString() + "_" + model.Image3.FileName;
                string filepath = Path.Combine(uploadfolder, UniqueFileNameimage3);
                using (var stream = System.IO.File.Create(filepath))
                {
                    await model.Image3.CopyToAsync(stream);
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
                Image2 = UniqueFileNameimage2,
                Image3 = UniqueFileNameimage3,
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
            string UniqueFileNameimage2 = null;
            if (model.ImageNo2)
            {
                if (model.Image2 != null)
                {
                    string uploadfolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets/images");
                    UniqueFileNameimage2 = Guid.NewGuid().ToString() + "_" + model.Image2.FileName;
                    string filepath = Path.Combine(uploadfolder, UniqueFileNameimage2);
                    if (product.Image2 != null)
                    {
                        string prevFile = Path.Combine(uploadfolder, product.Image2);
                        System.IO.File.Delete(prevFile);
                    }
                    using (var stream = System.IO.File.Create(filepath))
                    {
                        await model.Image2.CopyToAsync(stream);
                    }
                }
            }
            
            string UniqueFileNameimage3 = null;
            if (model.ImageNo3)
            {
                if (model.Image3 != null)
                {
                    string uploadfolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets/images");
                    UniqueFileNameimage3 = Guid.NewGuid().ToString() + "_" + model.Image3.FileName;
                    string filepath = Path.Combine(uploadfolder, UniqueFileNameimage3);
                    if(product.Image3 != null)
                    {
                        string prevFile = Path.Combine(uploadfolder, product.Image3);
                        System.IO.File.Delete(prevFile);
                    }
                    using (var stream = System.IO.File.Create(filepath))
                    {
                        await model.Image3.CopyToAsync(stream);
                    }
                }
            }
            product.Name = model.Name;
            product.Description = model.Description;
            product.Vendor = model.Vendor;
            product.Price = model.Price;
            product.PriceBeforeDiscount = model.PriceBeforeDiscount;
            product.Image1 = (UniqueFileNameimage1 != null) ? UniqueFileNameimage1 : product.Image1;
            product.Image2 = (UniqueFileNameimage2 != null) ? UniqueFileNameimage2 : product.Image2;
            product.Image3 = (UniqueFileNameimage3 != null) ? UniqueFileNameimage3 : product.Image3;
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
            if (products.Image2 != null)
            {
                string uploadfolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets/images");
                string filepath = Path.Combine(uploadfolder, products.Image2);
                System.IO.File.Delete(filepath);
            }
            if (products.Image3 != null)
            {
                string uploadfolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets/images");
                string filepath = Path.Combine(uploadfolder, products.Image3);
                System.IO.File.Delete(filepath);
            }
            _context.Products.Remove(products);
            await _context.SaveChangesAsync();
            return products;
        }
    }
}
