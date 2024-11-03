using Models.Entity;

namespace WebMVC.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<IEnumerable<ResearchProduct>> GetAll();
        Task<ResearchProduct?> GetById(int id);
        Task<ResearchProduct> Create(ResearchProduct researchProduct);
        Task<ResearchProduct?> Update(ResearchProduct researchProduct);
        Task<ResearchProduct?> Delete(int id);
        Task<ResearchProduct?> GetByUrlHandle(string urlHandle);
    }
}
