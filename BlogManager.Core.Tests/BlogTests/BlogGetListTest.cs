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

public class BlogGetListTest
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
    public async Task BlogGetListTest_MustReturnCorrectListOfData_NotIncludeAuthorInfo()
    {
        var handler               = new GetBlogListQueryHandler(new BlogRepository(dbContext), mockLogger.Object);
        var result                = await handler.Handle(new GetBlogListQuery(false), new CancellationToken());
        var blogListFromDbContext = await dbContext.Blogs.ToListAsync();
        result.Should().NotBeNull();
        result.Should().NotBeNull();
        result.Any(b => b.Author != null).Should().BeFalse();
        result.Count.Should().Be(blogListFromDbContext.Count);
        result.Any(b => b.Title == "Test Title 1").Should().BeTrue();
        result.Any(b => b.Description == "Test Description 4").Should().BeTrue();
    }

    [Test]
    public async Task BlogGetListTest_MustReturnCorrectListOfData_IncludeAuthorInfo()
    {
        var handler               = new GetBlogListQueryHandler(new BlogRepository(dbContext), new SerilogAdapter());
        var result                = await handler.Handle(new GetBlogListQuery(true), new CancellationToken());
        var blogListFromDbContext = await dbContext.Blogs.Include(b => b.Author).ToListAsync();
        result.Should().NotBeNull();
        result.Should().NotBeNull();
        result.All(b => b.Author != null).Should().BeTrue();
        result.Count.Should().Be(blogListFromDbContext.Count);
        result.Any(b => b.Title == "Test Title 1").Should().BeTrue();
        result.Any(b => b.Description == "Test Description 4").Should().BeTrue();
    }
}