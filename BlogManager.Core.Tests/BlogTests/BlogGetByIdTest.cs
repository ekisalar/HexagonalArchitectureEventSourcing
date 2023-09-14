using BlockManager.Tests.Shared;
using BlogManager.Adapter.Logger;
using BlogManager.Adapter.PostgreSQL.DbContext;
using BlogManager.Adapter.PostgreSQL.Repositories;
using BlogManager.Core.Handlers.QueryHandlers;
using BlogManager.Core.Logger;
using BlogManager.Core.Queries;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BlogManager.Core.Tests.BlogTests;

public class BlogGetByIdTest
{
    private IBlogDbContext           dbContext;
    private Mock<IBlogManagerLogger> mockLogger;


    [SetUp]
    public async Task Setup()
    {
        dbContext  = await DbContextFactory.CreatePostgreSqlInMemoryDbContext();
        mockLogger = new Mock<IBlogManagerLogger>();

    }

    [Test]
    public async Task BlogGeByIdTest_MustReturnCorrectBlog()
    {
        var handler           = new GetBlogByIdQueryHandler(new BlogRepository(dbContext), mockLogger.Object);
        var blogFromDbContext = await dbContext.Blogs.FirstAsync();

        var result = await handler.Handle(new GetBlogByIdQuery(blogFromDbContext.Id), CancellationToken.None);
        result.Should().NotBeNull();
        result.Id.Should().Be(blogFromDbContext.Id);
    }

    [Test]
    public async Task BlogGetByIdTest_MustReturnCorrectBlogIncludeAuthorInfo()
    {
        var handler           = new GetBlogByIdQueryHandler(new BlogRepository(dbContext), new SerilogAdapter());
        var blogFromDbContext = await dbContext.Blogs.Include(b => b.Author).FirstAsync();

        var result = await handler.Handle(new GetBlogByIdQuery(blogFromDbContext.Id, true), CancellationToken.None);
        result.Should().NotBeNull();
        result.Id.Should().Be(blogFromDbContext.Id);
        result.AuthorId.Should().Be(blogFromDbContext.AuthorId);
        result.Content.Should().Be(blogFromDbContext.Content);
        result.Author.Should().NotBeNull();
        result.Author.Id.Should().Be(blogFromDbContext.Author.Id);
        result.Author.Name.Should().Be(blogFromDbContext.Author.Name);
        result.Author.Surname.Should().Be(blogFromDbContext.Author.Surname);
    }
    
    [Test]
    public async Task BlogGeByIdTest_MustReturnNullForNonExistingBlogId()
    {
        var handler           = new GetBlogByIdQueryHandler(new BlogRepository(dbContext), new SerilogAdapter());
        var result = await handler.Handle(new GetBlogByIdQuery(Guid.NewGuid()), CancellationToken.None);
        result.Should().BeNull();
    }
}