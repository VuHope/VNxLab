using Models.Entity;

namespace WebMVC.Repository.IRepository
{
    public interface IPortfolioRepository
    {
        IEnumerable<Portfolio> GetAllPortfolios(string category = null, string sortOrder = null);
        Portfolio GetPortfolioById(int id);
        void AddPortfolio(Portfolio portfolio);
        void UpdatePortfolio(Portfolio portfolio);
        void DeletePortfolio(int id);
    }
}
