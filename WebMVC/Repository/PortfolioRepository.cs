using Microsoft.EntityFrameworkCore;
using Models.Entity;
using WebMVC.Data;
using WebMVC.Repository.IRepository;

namespace WebMVC.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private List<Portfolio> portfolios = new List<Portfolio>
        {
            new Portfolio { Id = 1, Title = "Project A", Description = "Mô tả dự án A", Category = "Design", Popularity = 150, Images = new List<string> { "/images/projectA1.jpg" }, DateCreated = DateTime.Now },
            new Portfolio { Id = 2, Title = "Project B", Description = "Mô tả dự án B", Category = "Web Development", Popularity = 200, Images = new List<string> { "/images/projectB1.jpg" }, DateCreated = DateTime.Now }
        };

        public IEnumerable<Portfolio> GetAllPortfolios(string category = null, string sortOrder = null)
        {
            var query = portfolios.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            if (sortOrder == "popularity")
            {
                query = query.OrderByDescending(p => p.Popularity);
            }

            return query.ToList();
        }

        public Portfolio GetPortfolioById(int id)
        {
            return portfolios.FirstOrDefault(p => p.Id == id);
        }

        public void AddPortfolio(Portfolio portfolio)
        {
            portfolio.Id = portfolios.Max(p => p.Id) + 1;
            portfolios.Add(portfolio);
        }

        public void UpdatePortfolio(Portfolio portfolio)
        {
            var existingPortfolio = GetPortfolioById(portfolio.Id);
            if (existingPortfolio != null)
            {
                existingPortfolio.Title = portfolio.Title;
                existingPortfolio.Description = portfolio.Description;
                existingPortfolio.Category = portfolio.Category;
                existingPortfolio.Popularity = portfolio.Popularity;
                existingPortfolio.Images = portfolio.Images;
                existingPortfolio.VideoUrl = portfolio.VideoUrl;
            }
        }

        public void DeletePortfolio(int id)
        {
            var portfolio = GetPortfolioById(id);
            if (portfolio != null)
            {
                portfolios.Remove(portfolio);
            }
        }
    }
}
