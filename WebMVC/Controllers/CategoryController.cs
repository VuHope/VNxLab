using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entity;
using System.ComponentModel;
using WebMVC.Repository;
using WebMVC.Repository.IRepository;
using WebMVC.ViewModels;

namespace WebMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryController(ICategoryRepository categoryRepository, IWebHostEnvironment webHostEnvironment)
        {
            _categoryRepository = categoryRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAll();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            var result = await _categoryRepository.Create(category);
            if (result != null)
            {
                TempData["Success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var data = await _categoryRepository.GetById(id);
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Category category)
        {
            var result = await _categoryRepository.Update(category);
            if (result != null)
            {
                TempData["Success"] = "Categỏy updated successfully";
                return RedirectToAction("Index");
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
            var data = await _categoryRepository.GetById(id);
            if (data == null)
            {
                return NotFound();
            }
            var result = await _categoryRepository.Delete(id);
            if (result == null)
            {
                return NotFound();
            }
            TempData["Success"] = "News deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
