using BlogManager.Adapter.PostgreSQL.DbContext;
using BlogManager.Core.Domain;
using BlogManager.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BlogManager.Adapter.PostgreSQL.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly IBlogDbContext _dbContext;

    public AuthorRepository(IBlogDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<Author?> GetAuthorByIdAsync(Guid id, bool asNoTracking = true)
    {
        var query = _dbContext.Authors.AsQueryable();
        if (asNoTracking)
            query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<List<Author>?> GetAllAuthorsAsync(bool asNoTracking = true)
    {
        var query = _dbContext.Authors.AsQueryable();
        if (asNoTracking)
            query = query.AsNoTracking();
        return await query.ToListAsync();
    }

    public async Task<Author> AddAuthorAsync(Author author)
    {
        var result = await _dbContext.Authors.AddAsync(author);
        await _dbContext.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<Author> UpdateAsync(Author author)
    {
        var result = _dbContext.Authors.Update(author);
        await _dbContext.SaveChangesAsync();
        return result.Entity;
    }

    public async Task DeleteAuthorAsync(Author author)
    {
        _dbContext.Authors.Remove(author);
        await _dbContext.SaveChangesAsync();
    }
}