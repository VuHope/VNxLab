using Models.Entity;

namespace WebMVC.Repository.IRepository
{
    public interface INewsRepository
    {
        Task<IEnumerable<News>> GetAll();
        Task<News?> GetById(int id);
        Task<News> CreateNews(News news);
        Task<News?> UpdateNews(News news);
        Task<News?> DeleteNews(int id);

        Task<News?> GetByUrlHandle(string urlHandle);

    }
}
