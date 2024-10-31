using Microsoft.EntityFrameworkCore;
using Models.Entity;
using WebMVC.Data;
using WebMVC.Repository.IRepository;

namespace WebMVC.Repository
{
    public class NewsRepository : INewsRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public NewsRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<News> CreateNews(News news)
        {
            news.DatePublished = DateTime.UtcNow;
            await _dbContext.News.AddAsync(news);
            await _dbContext.SaveChangesAsync();
            return news;
        }

        public async Task<News?> DeleteNews(int id)
        {
            var result = await _dbContext.News.FirstOrDefaultAsync(n => n.Id == id);
            if (result != null)
            {
                _dbContext.News.Remove(result);
                await _dbContext.SaveChangesAsync();
                return result;
            }
            return null;
        }

        public async Task<News?> GetById(int id)
        {
            var result = await _dbContext.News.FirstOrDefaultAsync(n => n.Id == id);
            if(result != null)
            {
                return result;
            }
            return null;
        }

        public async Task<IEnumerable<News>> GetAll()
        {
            return await _dbContext.News.ToListAsync();
        }

        public async Task<News?> UpdateNews(News news)
        {
            var result = _dbContext.News.FirstOrDefault(n => n.Id == news.Id);
            if (result != null)
            {
                result.Heading = news.Heading;
                result.PageTitle = news.PageTitle;
                result.Content = news.Content;
                result.ShortDescription = news.ShortDescription;
                result.UrlHandle = news.UrlHandle;
                if(news.NewsImage != null)
                {
                    result.NewsImage = news.NewsImage;
                }
                await _dbContext.SaveChangesAsync();
                return result;
            }
            return null;
        }

        public async Task<News?> GetByUrlHandle(string urlHandle)
        {
           var result = await _dbContext.News.FirstOrDefaultAsync(n => n.UrlHandle == urlHandle);
            if (result != null)
            {
                return result;
            }
            return null;
        }
    }
}
