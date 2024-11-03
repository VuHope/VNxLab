using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Entity;
using WebMVC.Repository;
using WebMVC.Repository.IRepository;
using WebMVC.ViewModels;

namespace WebMVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductImage _productImageRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductController(IProductRepository productRepository, IProductImage productImageRepository, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        // GET: ProductController
        public async Task<IActionResult> List()
        {
            var data = await _productRepository.GetAll();
            return View(data);
        }

        // GET: ProductController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var data = await _productRepository.GetById(id); 
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }


        // GET: ProductController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel productViewModel, IFormFile? file)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            productViewModel.ResearchProduct.UserId = userId;

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string newsPath = Path.Combine(wwwRootPath, @"images\product");

                using (var fileStream = new FileStream(Path.Combine(newsPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                productViewModel.ResearchProduct.CoverImg = @"\images\product\" + fileName;
            }

            var result = await _productRepository.Create(productViewModel.ResearchProduct);
            if (result != null)
            {
                return RedirectToAction("List");
            }
            return View(productViewModel);
        }

        // GET: ProductController/Edit/5
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            var productViewModel = new ProductViewModel
            {
                ResearchProduct = product
            };

            return View(productViewModel);
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProductViewModel productViewModel, IFormFile? file)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string newsPath = Path.Combine(wwwRootPath, @"images\product");
                if (!string.IsNullOrEmpty(productViewModel.ResearchProduct.CoverImg))
                {
                    var oldImagePath =
                        Path.Combine(wwwRootPath, productViewModel.ResearchProduct.CoverImg.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                using (var fileStream = new FileStream(Path.Combine(newsPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                productViewModel.ResearchProduct.CoverImg = @"\images\product\" + fileName;
            }

            var result = await _productRepository.Update(productViewModel.ResearchProduct);
            if (result != null)
            {
                return RedirectToAction("List");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            var result = await _productRepository.Delete(id);
            if (result == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(product.CoverImg))
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                var imagePath = Path.Combine(wwwRootPath, product.CoverImg.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            return RedirectToAction("List");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var productList = await _productRepository.GetAll();
            var productViewModel = new ProductViewModel
            {
                ListResearchProduct = productList.ToList()
            };

            foreach (var product in productViewModel.ListResearchProduct)
            {
                var user = await _userManager.FindByIdAsync(product.UserId);
                if (user != null)
                {
                    ViewData[$"AuthorEmail_{product.Id}"] = user.Email;
                }
            }

            return View(productViewModel);
        }

        [Authorize]
        public async Task<IActionResult> MyWorks()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userProducts = await _productRepository.GetAll();
            var userWorks = userProducts.Where(p => p.UserId == userId).ToList();

            foreach (var product in userWorks)
            {
                var user = await _userManager.FindByIdAsync(product.UserId);
                if (user != null)
                {
                    ViewData[$"AuthorEmail_{product.Id}"] = user.Email;
                }
            }

            var productViewModel = new ProductViewModel
            {
                ListResearchProduct = userWorks
            };

            return View(productViewModel);
        }



        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> DisplayOneProduct(string urlHandle)
        {
            var result = await _productRepository.GetByUrlHandle(urlHandle);
            ProductViewModel productViewModel = new ProductViewModel
            {
                ResearchProduct = result
            };
            return View(productViewModel);
        }



        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> DisplayProductImages(int id)
        {
            var product = await _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            // Lấy tất cả ảnh liên quan đến sản phẩm từ ProductImageRepository
            var productImages = await _productImageRepository.GetByProductId(product.Id);

            var productViewModel = new ProductViewModel
            {
                ResearchProduct = product,
                ProductImages = productImages.ToList()
            };

            return View(productViewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> DisplayProductImagesReadOnly(int id)
        {
            var product = await _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            // Lấy tất cả ảnh liên quan đến sản phẩm từ ProductImageRepository
            var productImages = await _productImageRepository.GetByProductId(product.Id);

            var productViewModel = new ProductViewModel
            {
                ResearchProduct = product,
                ProductImages = productImages.ToList()
            };

            return View(productViewModel);
        }

        // POST: Product/UploadImage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(ProductViewModel model, IFormFile file)
        {
            if (file != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string path = Path.Combine(wwwRootPath, "images/product", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var productImage = new ProductImage
                {
                    ProductId = model.ResearchProduct.Id,
                    ImagePath = "/images/product/" + fileName
                };

                await _productImageRepository.Create(productImage);
            }
            return RedirectToAction("DisplayProductImages", new { id = model.ResearchProduct.Id });
        }

        // POST: Product/DeleteImage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int id, int productId)
        {
            var productImage = await _productImageRepository.GetById(id);
            if (productImage != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                var imagePath = Path.Combine(wwwRootPath, productImage.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                await _productImageRepository.Delete(id);
            }
            return RedirectToAction("DisplayProductImages", new { id = productId });
        }
    }
}
