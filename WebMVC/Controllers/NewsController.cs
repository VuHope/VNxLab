using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entity;
using WebMVC.Repository.IRepository;
using WebMVC.ViewModels;

namespace WebMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class NewsController : Controller
    {
        private readonly INewsRepository _newsRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NewsController(INewsRepository newsRepository, IWebHostEnvironment webHostEnvironment)
        {
            _newsRepository = newsRepository;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var news = await _newsRepository.GetAll();
            return View(news);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsViewModel newsMV, IFormFile? file)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string newsPath = Path.Combine(wwwRootPath, @"images\news");

                using (var fileStream = new FileStream(Path.Combine(newsPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                newsMV.News.NewsImage = @"\images\news\" + fileName;
            }

            var result = await _newsRepository.CreateNews(newsMV.News);
            if (result != null)
            {
                return RedirectToAction("List");
            }
            return View(newsMV);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var news = await _newsRepository.GetById(id);
            if (news == null)
            {
                return NotFound();
            }

            var newsViewModel = new NewsViewModel
            {
                News = news
            };

            return View(newsViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(NewsViewModel newsMV, IFormFile? file)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string newsPath = Path.Combine(wwwRootPath, @"images\news");
                if (!string.IsNullOrEmpty(newsMV.News.NewsImage))
                {
                    //delete the old image
                    var oldImagePath =
                        Path.Combine(wwwRootPath, newsMV.News.NewsImage.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                using (var fileStream = new FileStream(Path.Combine(newsPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                newsMV.News.NewsImage = @"\images\news\" + fileName;
            }

            var result = await _newsRepository.UpdateNews(newsMV.News);
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
            var news = await _newsRepository.GetById(id);
            if (news == null)
            {
                return NotFound();
            }
            var result = await _newsRepository.DeleteNews(id);
            if (result == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(news.NewsImage))
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                var imagePath = Path.Combine(wwwRootPath, news.NewsImage.TrimStart('\\'));
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
            var newsList = await _newsRepository.GetAll();
            var viewModel = new NewsViewModel
            {
                ListNews = newsList.ToList()
            };
            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> DisplayOneNews(string urlHandle)
        {
            var result = await _newsRepository.GetByUrlHandle(urlHandle);
            NewsViewModel newsViewModel = new NewsViewModel
            {
                News = result
            };
            return View(newsViewModel);
        }
    }
}
