using Microsoft.EntityFrameworkCore;
using Models.Entity;
using WebMVC.Data;
using WebMVC.Repository.IRepository;

namespace WebMVC.Repository
{
    public class ResearchProductComment : IResearchProductComment
    {
        private readonly ApplicationDbContext _dbContext;

        public ResearchProductComment(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Comment> AddAsync(Comment comment)
        {
            _dbContext.Comments.Add(comment);
            await _dbContext.SaveChangesAsync();
            return comment;
        }

        public async Task<IEnumerable<Comment>> GetByIdAsync(int Id)
        {
            return await _dbContext.Comments.Where(c => c.ProductId == Id).ToListAsync();
        }
    }
}
