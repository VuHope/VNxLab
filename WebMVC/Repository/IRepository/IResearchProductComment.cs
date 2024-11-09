using Models.Entity;

namespace WebMVC.Repository.IRepository
{
    public interface IResearchProductComment
    {
        Task<Comment> AddAsync(Comment comment);
        Task<IEnumerable<Comment>> GetByIdAsync(int Id);
    }
}
