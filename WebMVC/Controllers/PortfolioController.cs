using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Models.Entity;
using WebMVC.Data;
using WebMVC.Utility;
using WebMVC.ViewModels;
using WebMVC.Repository.IRepository;

namespace WebMVC.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductRepository _productRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public PortfolioController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IProductRepository productRepository)
        {
            _context = context;
            _userManager = userManager;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _productRepository.GetAll();
            var userIdsWithProducts = users
                .GroupBy(p => p.UserId)
                .Where(g => g.Count() >= 1) 
                .Select(g => g.Key)
                .ToList();

            var usersInRole = await _userManager.GetUsersInRoleAsync(SD.User);
            var usersWithProducts = usersInRole
                .Where(user => userIdsWithProducts.Contains(user.Id))
                .ToList();

            return View(usersWithProducts);
        }


        public async Task<IActionResult> UserPortfolio(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userProducts = await _productRepository.GetAll();
            var userWorks = userProducts.Where(p => p.UserId == userId).ToList();

            var productViewModel = new ProductViewModel
            {
                ListResearchProduct = userWorks
            };

            ViewData["UserName"] = user.FullName;
            return View(productViewModel);
        }

    }
}
