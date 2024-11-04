using Models.Entity;

namespace WebMVC.Repository.IRepository
{
    public interface IPortfolioRepository
    {
        Task<IEnumerable<Portfolio>> GetAllPortfoliosAsync(string category = null, bool sortByPopularity = false);
        Task<Portfolio> GetPortfolioByIdAsync(int id);
    }
}
