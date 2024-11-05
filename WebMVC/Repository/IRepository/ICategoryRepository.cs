using Models.Entity;

namespace WebMVC.Repository.IRepository
{
    public interface ICategoryRepository
    {
        Task<Category> Create(Category category);
        Task<Category?> Update(Category category);
        Task<Category?> Delete(int id);
        Task<Category?> GetById(int id);
        Task<IEnumerable<Category>> GetAll();
        Task<IEnumerable<Category>> GetCategoriesByProductId(int productId);
    }
}
