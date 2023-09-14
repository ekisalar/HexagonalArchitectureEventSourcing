using BlogManager.Adapter.PostgreSQL.DbContext;
using BlogManager.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace BlockManager.Tests.Shared;

public static class DbContextFactory
{
    public static async Task<BlogDbContext> CreatePostgreSqlInMemoryDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<BlogDbContext>()
           .UseInMemoryDatabase("PostgreSqlInMemoryDatabase");

        var dbContext = new BlogDbContext(optionsBuilder.Options);
        await GenerateInitialDataAsync(dbContext);
        return dbContext;
    }

    private static async Task GenerateInitialDataAsync(BlogDbContext dbContext)
    {
        await dbContext.Authors.AddRangeAsync(await Author.CreateAsync(Guid.NewGuid(), "TestName 1", "TestSurname 1"),
                                              await Author.CreateAsync(Guid.NewGuid(), "TestName 2", "TestSurname 2"),
                                              await Author.CreateAsync(Guid.NewGuid(), "TestName 3", "TestSurname 3"),
                                              await Author.CreateAsync(Guid.NewGuid(), "TestName 4", "TestSurname 4"),
                                              await Author.CreateAsync(Guid.NewGuid(), "TestName 5", "TestSurname 5"));
        await dbContext.SaveChangesAsync();

        var authorIds = dbContext.Authors.Select(a => a.Id).ToArray();

        await dbContext.Blogs.AddRangeAsync(await Blog.CreateAsync(Guid.NewGuid(), authorIds[0], "Test Title 1", "Test Description 1", "Test Content 1 "),
                                            await Blog.CreateAsync(Guid.NewGuid(), authorIds[1], "Test Title 2", "Test Description 2", "Test Content 2"),
                                            await Blog.CreateAsync(Guid.NewGuid(), authorIds[2], "Test Title 3", "Test Description 3", "Test Content 3"),
                                            await Blog.CreateAsync(Guid.NewGuid(), authorIds[3], "Test Title 4", "Test Description 4", "Test Content 4"),
                                            await Blog.CreateAsync(Guid.NewGuid(), authorIds[4], "Test Title 4", "Test Description 4", "Test Content 4"));
        await dbContext.SaveChangesAsync();
    }
}