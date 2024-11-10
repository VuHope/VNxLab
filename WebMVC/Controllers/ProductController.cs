using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Models.Entity;
using WebMVC.Repository;
using WebMVC.Repository.IRepository;
using WebMVC.ViewModels;

namespace WebMVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductImage _productImageRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IResearchProductComment _researchProductComment;

        private readonly SignInManager<ApplicationUser> _signInManager;
        public ProductController(IProductRepository productRepository, IProductImage productImageRepository, 
            IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager, 
            ICategoryRepository categoryRepository, IResearchProductComment researchProductComment, 
            SignInManager<ApplicationUser> signInManager)
        {
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
            _researchProductComment = researchProductComment;
            _signInManager = signInManager;
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
            var product = await _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            // Lấy danh sách các Category liên kết với sản phẩm
            var categories = await _categoryRepository.GetCategoriesByProductId(id);

            var productViewModel = new ProductViewModel
            {
                ResearchProduct = product,
                Categories = categories.ToList()
            };

            return View(productViewModel);
        }



        // GET: ProductController/Create
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAll();
            var productViewModel = new ProductViewModel
            {
                Categories = categories.ToList()
            };
            return View(productViewModel);
        }


        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel productViewModel, IFormFile? file)
        {
            // Lấy thông tin người dùng hiện tại từ Identity
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            productViewModel.ResearchProduct.UserId = user.Id;

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string newsPath = Path.Combine(wwwRootPath, @"images\product");

                if (!Directory.Exists(newsPath))
                {
                    Directory.CreateDirectory(newsPath);
                }

                using (var fileStream = new FileStream(Path.Combine(newsPath, fileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                productViewModel.ResearchProduct.CoverImg = @"\images\product\" + fileName;
            }

            var result = await _productRepository.Create(productViewModel.ResearchProduct);
            if (result != null)
            {
                foreach (var categoryId in productViewModel.SelectedCategoryIds)
                {
                    await _productRepository.AddCategoryToProduct(result.Id, categoryId);
                }
                return RedirectToAction("MyWorks");
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

            var categories = await _categoryRepository.GetAll();
            var selectedCategories = await _categoryRepository.GetCategoriesByProductId(id);

            var productViewModel = new ProductViewModel
            {
                ResearchProduct = product,
                Categories = categories.ToList(),
                SelectedCategoryIds = selectedCategories.Select(c => c.Id).ToList()
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
                // Cập nhật các Category liên kết
                var existingCategories = await _categoryRepository.GetCategoriesByProductId(result.Id);
                var existingCategoryIds = existingCategories.Select(c => c.Id).ToList();

                // Thêm cate
                var categoriesToAdd = productViewModel.SelectedCategoryIds.Except(existingCategoryIds).ToList();
                foreach (var categoryId in categoriesToAdd)
                {
                    await _productRepository.AddCategoryToProduct(result.Id, categoryId);
                }

                // Xóa cate
                var categoriesToRemove = existingCategoryIds.Except(productViewModel.SelectedCategoryIds).ToList();
                foreach (var categoryId in categoriesToRemove)
                {
                    await _productRepository.RemoveCategoryFromProduct(result.Id, categoryId);
                }


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
            // Lấy thông tin người dùng hiện tại từ Identity
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = user.Id;
            var userProducts = await _productRepository.GetAll();
            var userWorks = userProducts.Where(p => p.UserId == userId).ToList();

            foreach (var product in userWorks)
            {
                var author = await _userManager.FindByIdAsync(product.UserId);
                if (author != null)
                {
                    ViewData[$"AuthorEmail_{product.Id}"] = author.Email;
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

            // Lấy tất cả các category của product
            var allCategories = await _categoryRepository.GetAll();

            // Lấy các Category đã được liên kết với sản phẩm
            var selectedCategories = await _categoryRepository.GetCategoriesByProductId(product.Id);

            var productViewModel = new ProductViewModel
            {
                ResearchProduct = product,
                ProductImages = productImages.ToList(),
                Categories = allCategories.ToList(), 
                SelectedCategoryIds = selectedCategories.Select(c => c.Id).ToList() 
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

            // Lấy tất cả các category của product
            var categories = await _categoryRepository.GetCategoriesByProductId(product.Id);

            var researchProductComments = await _researchProductComment.GetByIdAsync(product.Id);

            var researchProduct = new List<CommentViewModel>();

            foreach (var item in researchProductComments)
            {
                researchProduct.Add(new CommentViewModel
                {
                    Content = item.Content,
                    CreatedAt = item.CreatedAt,
                    UserName = (await _userManager.FindByIdAsync(item.UserId.ToString())).UserName
                });


            }
            var productViewModel = new ProductViewModel
            {
                ResearchProduct = product,
                ProductImages = productImages.ToList(),
                Categories = categories.ToList(),
                Comment = researchProduct
            };

            return View(productViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisplayProductImagesReadOnly(ProductViewModel model)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var domainModel = new Comment
                {
                    UserId = _userManager.GetUserId(User),
                    ProductId = model.ResearchProduct.Id,
                    Content = model.CommentContent,
                    CreatedAt = DateTime.Now
                };
                
                await _researchProductComment.AddAsync(domainModel);
                return RedirectToAction("DisplayProductImagesReadOnly", "Product", new { urlHandle = model.ResearchProduct.UrlHandle });
            }
            return View();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProductCategories(int productId, List<int> selectedCategoryIds)
        {
            var existingCategories = await _categoryRepository.GetCategoriesByProductId(productId);
            var existingCategoryIds = existingCategories.Select(c => c.Id).ToList();

            // Thêm category mới
            var categoriesToAdd = selectedCategoryIds.Except(existingCategoryIds).ToList();
            foreach (var categoryId in categoriesToAdd)
            {
                await _productRepository.AddCategoryToProduct(productId, categoryId);
            }

            // Xóa category không còn chọn
            var categoriesToRemove = existingCategoryIds.Except(selectedCategoryIds).ToList();
            foreach (var categoryId in categoriesToRemove)
            {
                await _productRepository.RemoveCategoryFromProduct(productId, categoryId);
            }

            return RedirectToAction("DisplayProductImages", new { id = productId });
        }


    }
}
