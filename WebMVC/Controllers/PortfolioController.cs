using Microsoft.AspNetCore.Mvc;
using WebMVC.Data;

namespace WebMVC.Controllers
{
    using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("category/{category?}/popularity/{sortByPopularity?}")]
        public async Task<IActionResult> Index(string category = null, bool sortByPopularity = false)
        {
            var portfolios = await _portfolioRepository.GetAllPortfoliosAsync(category, sortByPopularity);
            return View(portfolios);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var portfolio = await _portfolioRepository.GetPortfolioByIdAsync(id);
            if (portfolio == null)
            {
                return NotFound();
            }
            return View(portfolio);
        }
    }


}
