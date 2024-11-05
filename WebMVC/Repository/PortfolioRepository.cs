using Microsoft.EntityFrameworkCore;
using Models.Entity;
using WebMVC.Data;
using WebMVC.Repository.IRepository;

namespace WebMVC.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDbContext _context;

        public PortfolioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Portfolio>> GetAllPortfoliosAsync(string category = null, bool sortByPopularity = false)
        {
            var portfolios = _context.Portfolios.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                portfolios = portfolios.Where(p => p.Category == category);
            }

            if (sortByPopularity)
            {
                portfolios = portfolios.OrderByDescending(p => p.Popularity);
            }

            return await portfolios.ToListAsync();
        }

        public async Task<Portfolio> GetPortfolioByIdAsync(int id)
        {
            return await _context.Portfolios
                                 .Include(p => p.Images) 
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
