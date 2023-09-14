using BlogManager.Core.Domain;

namespace BlogManager.Core.Repositories;

public interface IBlogRepository
{
    Task<Blog?>       GetBlogByIdAsync(Guid id,                        bool includeAuthorInfo, bool asNoTracking = true);
    Task<List<Blog>?> GetAllBlogsAsync(bool includeAuthorInfo = false, bool asNoTracking = true);
    Task<Blog>        AddBlogAsync(Blog     blog);
    Task<Blog>        UpdateAsync(Blog      blog);

    Task DeleteBlogAsync(Blog blog);
}