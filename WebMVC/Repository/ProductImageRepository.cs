using Microsoft.EntityFrameworkCore;
using Models.Entity;
using WebMVC.Data;
using WebMVC.Repository.IRepository;

namespace WebMVC.Repository
{
    public class ProductImageRepository : IProductImage
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductImageRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProductImage> Create(ProductImage productImage)
        {
            await _dbContext.ProductImages.AddAsync(productImage);
            await _dbContext.SaveChangesAsync();
            return productImage;
        }

        public async Task<ProductImage?> Delete(int id)
        {
            var productImage = await _dbContext.ProductImages.FirstOrDefaultAsync(pi => pi.Id == id);
            if (productImage != null)
            {
                _dbContext.ProductImages.Remove(productImage);
                await _dbContext.SaveChangesAsync();
                return productImage;
            }
            return null;
        }

        public async Task<IEnumerable<ProductImage>> GetAll()
        {
            return await _dbContext.ProductImages.ToListAsync();
        }

        public async Task<ProductImage?> GetById(int id)
        {
            return await _dbContext.ProductImages.FirstOrDefaultAsync(pi => pi.Id == id);
        }

        public async Task<IEnumerable<ProductImage>> GetByProductId(int productId)
        {
            return await _dbContext.ProductImages.Where(pi => pi.ProductId == productId).ToListAsync();
        }
    }
}
