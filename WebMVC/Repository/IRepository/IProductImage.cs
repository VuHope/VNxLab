using Models.Entity;

namespace WebMVC.Repository.IRepository
{
    public interface IProductImage
    {
        Task<ProductImage> Create(ProductImage productImage);
        Task<ProductImage?> Delete(int id);
        Task<IEnumerable<ProductImage>> GetAll();
        Task<ProductImage?> GetById(int id);
        Task<IEnumerable<ProductImage>> GetByProductId(int productId);
    }
}
