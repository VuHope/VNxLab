using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Models.Entity;
using WebMVC.Repository.IRepository;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace WebMVC.Controllers
{
    //[Authorize]
    public class ProductImageController : Controller
    {
        private readonly IProductImage _productImageRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductImageController(IProductImage productImageRepository, IWebHostEnvironment webHostEnvironment)
        {
            _productImageRepository = productImageRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: ProductImage/ListByProduct/5
        public async Task<IActionResult> ListByProduct(int productId)
        {
            var images = await _productImageRepository.GetByProductId(productId);
            return View(images);
        }

        // GET: ProductImage/Create
        public IActionResult Create(int productId)
        {
            var model = new ProductImage { ProductId = productId };
            return View(model);
        }

        // POST: ProductImage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductImage productImage, IFormFile file)
        {
            if (ModelState.IsValid && file != null)
            {
                // Tạo đường dẫn để lưu file trong thư mục wwwroot/images/product
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string path = Path.Combine(wwwRootPath, "images/product", fileName);

                // Lưu file vào thư mục
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Lưu đường dẫn file vào thuộc tính ImagePath
                productImage.ImagePath = "/images/product/" + fileName;

                // Lưu ảnh vào cơ sở dữ liệu
                await _productImageRepository.Create(productImage);

                return RedirectToAction("ListByProduct", new { productId = productImage.ProductId });
            }
            return View(productImage);
        }

        // POST: ProductImage/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int productId)
        {
            var productImage = await _productImageRepository.GetById(id);
            if (productImage != null)
            {
                // Xóa file ảnh khỏi thư mục wwwroot
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                var imagePath = Path.Combine(wwwRootPath, productImage.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                // Xóa ảnh khỏi cơ sở dữ liệu
                await _productImageRepository.Delete(id);
            }

            return RedirectToAction("ListByProduct", new { productId });
        }
    }
}
