using Microsoft.AspNetCore.Mvc;
using WebMVC.Data;
using System.Net;


namespace WebMVC.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Models.Entity;
    using System.Threading.Tasks;
    using WebMVC.Repository.IRepository;

    [Route("portfolio")]
    public class PortfolioController : Controller
    {
        private readonly IPortfolioRepository _portfolioRepository;

        public PortfolioController(IPortfolioRepository portfolioRepository)
        {
            _portfolioRepository = portfolioRepository;
        }

        [HttpGet("")]
        public IActionResult Index(string category, string sortOrder)
        {
            var portfolios = _portfolioRepository.GetAllPortfolios(category, sortOrder);
            return View(portfolios);
        }

        [HttpGet("{id}")]
        public IActionResult Details(int id)
        {
            var portfolio = _portfolioRepository.GetPortfolioById(id);
            if (portfolio == null)
            {
                return NotFound();
            }
            return View(portfolio);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("create")]
        public IActionResult Create(Portfolio portfolio)
        {
            if (ModelState.IsValid)
            {
                _portfolioRepository.AddPortfolio(portfolio);
                return RedirectToAction("Index");
            }
            return View(portfolio);
        }

        [HttpGet("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var portfolio = _portfolioRepository.GetPortfolioById(id);
            if (portfolio == null)
            {
                return NotFound();
            }
            return View(portfolio);
        }

        [HttpPost("edit/{id}")]
        public IActionResult Edit(int id, Portfolio portfolio)
        {
            if (ModelState.IsValid)
            {
                _portfolioRepository.UpdatePortfolio(portfolio);
                return RedirectToAction("Index");
            }
            return View(portfolio);
        }

        [HttpPost("delete/{id}")]
        public IActionResult Delete(int id)
        {
            _portfolioRepository.DeletePortfolio(id);
            return RedirectToAction("Index");
        }
    }
}
