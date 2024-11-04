using Microsoft.EntityFrameworkCore;
using Models.Entity;
using WebMVC.Data;
using WebMVC.Repository.IRepository;

namespace WebMVC.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ResearchProduct> Create(ResearchProduct product)
        {
            product.CreatedAt = DateTime.UtcNow;
            await _dbContext.ResearchProducts.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<ResearchProduct?> Delete(int id)
        {
            var result = await _dbContext.ResearchProducts.FirstOrDefaultAsync(n => n.Id == id);
            if (result != null)
            {
                _dbContext.ResearchProducts.Remove(result);
                await _dbContext.SaveChangesAsync();
                return result;
            }
            return null;
        }

        public async Task<ResearchProduct?> GetById(int id)
        {
            var result = await _dbContext.ResearchProducts.FirstOrDefaultAsync(n => n.Id == id);
            if (result != null)
            {
                return result;
            }
            return null;
        }

        public async Task<IEnumerable<ResearchProduct>> GetAll()
        {
            return await _dbContext.ResearchProducts.ToListAsync();
        }

        public async Task<ResearchProduct?> Update(ResearchProduct product)
        {
            var result = _dbContext.ResearchProducts.FirstOrDefault(n => n.Id == product.Id);
            if (result != null)
            {
                result.Title = product.Title;
                result.Summary = product.Summary;
                result.Content = product.Content;
                if (product.CoverImg != null)
                {
                    result.CoverImg = product.CoverImg;
                }
                await _dbContext.SaveChangesAsync();
                return result;
            }
            return null;
        }

        public async Task<ResearchProduct?> GetByUrlHandle(string urlHandle)
        {
            var result = await _dbContext.ResearchProducts.FirstOrDefaultAsync(n => n.UrlHandle == urlHandle);
            if (result != null)
            {
                return result;
            }
            return null;
        }

        public async Task AddCategoryToProduct(int productId, int categoryId)
        {
            var productCategory = new ResearchProductCategory
            {
                ProductId = productId,
                CategoryId = categoryId
            };
            _dbContext.ResearchProductCategories.Add(productCategory);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveCategoryFromProduct(int productId, int categoryId)
        {
            var productCategory = await _dbContext.ResearchProductCategories
                .FirstOrDefaultAsync(pc => pc.ProductId == productId && pc.CategoryId == categoryId);

            if (productCategory != null)
            {
                _dbContext.ResearchProductCategories.Remove(productCategory);
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}
