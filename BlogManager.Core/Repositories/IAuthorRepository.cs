using BlogManager.Core.Domain;

namespace BlogManager.Core.Repositories;

public interface IAuthorRepository
{
    Task<Author?>       GetAuthorByIdAsync(Guid  id, bool asNoTracking = true);
    Task<List<Author>?> GetAllAuthorsAsync(bool  asNoTracking = true);
    Task<Author>        AddAuthorAsync(Author    author);
    Task<Author>        UpdateAsync(Author       author);
    Task                DeleteAuthorAsync(Author author);
}