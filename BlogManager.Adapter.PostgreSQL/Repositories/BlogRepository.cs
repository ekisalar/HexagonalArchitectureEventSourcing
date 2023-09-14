using BlogManager.Adapter.PostgreSQL.DbContext;
using BlogManager.Core.Domain;
using BlogManager.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BlogManager.Adapter.PostgreSQL.Repositories;

public class BlogRepository : IBlogRepository
{
    private readonly IBlogDbContext _dbContext;

    public BlogRepository(IBlogDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<Blog?> GetBlogByIdAsync(Guid id, bool includeAuthorInfo, bool asNoTracking = true)
    {
        var query = _dbContext.Blogs.AsQueryable();
        if (asNoTracking)
            query = query.AsNoTracking();
        if (includeAuthorInfo)
            query = query.Include(b => b.Author);
        return await query.FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<List<Blog>?> GetAllBlogsAsync(bool includeAuthorInfo = false, bool asNoTracking = true)
    {
        var query = _dbContext.Blogs.AsQueryable();
        if (asNoTracking)
            query = query.AsNoTracking();
        if (includeAuthorInfo)
            query = query.Include(b => b.Author);
        return await query.ToListAsync();
    }

    public async Task<Blog> AddBlogAsync(Blog blog)
    {
        var result = await _dbContext.Blogs.AddAsync(blog);
        await _dbContext.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<Blog> UpdateAsync(Blog blog)
    {
        var result = _dbContext.Blogs.Update(blog);
        await _dbContext.SaveChangesAsync();
        return result.Entity;
    }

    public async Task DeleteBlogAsync(Blog blog)
    {
        _dbContext.Blogs.Remove(blog);
        await _dbContext.SaveChangesAsync();
    }
}