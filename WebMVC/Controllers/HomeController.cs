using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Diagnostics;
using System.Globalization;
using WebMVC.Repository.IRepository;
using WebMVC.ViewModels;

namespace WebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IProductRepository _productRepository;

        public HomeController(ILogger<HomeController> logger, IStringLocalizer<HomeController> localizer, IProductRepository productRepository)
        {
            _logger = logger;
            _localizer = localizer;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAll();
            ViewData["TheWorld"] = _localizer["TheWorld"];
            ViewData["BestCreator"] = _localizer["BestCreator"];
            ViewData["AreRight"] = _localizer["AreRight"];
            ViewData["HomeSmallText"] = _localizer["HomeSmallText"];
            ViewData["HomeButtonProduct"] = _localizer["HomeButtonProduct"];
            ViewData["HomeButtonAuthor"] = _localizer["HomeButtonAuthor"];
            return View(products);
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return LocalRedirect(returnUrl);
        }
    }
}
