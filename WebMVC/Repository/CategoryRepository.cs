using Microsoft.EntityFrameworkCore;
using Models.Entity;
using WebMVC.Data;
using WebMVC.Repository.IRepository;

namespace WebMVC.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Category> Create(Category category)
        {
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> Delete(int id)
        {
            var result = await _dbContext.Categories.FindAsync(id);
            if (result != null)
            {
                _dbContext.Categories.Remove(result);
                _dbContext.SaveChanges();
                return result;
            }
            return null;
        }

        public async Task<IEnumerable<Category>> GetAll()
        {
            return await _dbContext.Categories.ToListAsync();
        }

        public async Task<Category?> GetById(int id)
        {
            return await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Category>> GetCategoriesByProductId(int productId)
        {
            return await _dbContext.ResearchProductCategories
            .Where(rpc => rpc.ProductId == productId)
            .Include(rpc => rpc.Category) 
            .Select(rpc => rpc.Category)  
            .ToListAsync();
        }

        public async Task<Category?> Update(Category category)
        {
            var result = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == category.Id);
            if (result != null)
            {
                result.Name = category.Name;
                _dbContext.SaveChanges();
                return result;
            }
            return null;
        }
    }
}
