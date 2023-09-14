using BlockManager.Tests.Shared;
using BlogManager.Adapter.PostgreSQL.DbContext;
using BlogManager.Adapter.PostgreSQL.Repositories;
using BlogManager.Core.Commands.Blog;
using BlogManager.Core.Domain;
using BlogManager.Core.Events.Blog;
using BlogManager.Core.Handlers.CommandHandlers.Blog;
using BlogManager.Core.Handlers.EventHandlers;
using BlogManager.Core.Logger;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BlogManager.Core.Tests.BlogTests;

public class BlogDeleteTest
{
    private IBlogDbContext           _dbContext;
    private Mock<IBlogManagerLogger> _mockLogger;
    private Mock<IBlogManagerStreamHandler> _mockBlogManagerStream;
    private Blog                     _blogToDelete;


    [SetUp]
    public async Task Setup()
    {
        _dbContext             = await DbContextFactory.CreatePostgreSqlInMemoryDbContext();
        _mockLogger            = new Mock<IBlogManagerLogger>();
        _blogToDelete          = await _dbContext.Blogs.FirstAsync();
        _mockBlogManagerStream = new Mock<IBlogManagerStreamHandler>();
        _mockBlogManagerStream.Setup(s => s.HandleBlogDeletedEventAsync(It.IsAny<BlogDeletedEvent>()))
                              .Callback<BlogDeletedEvent>(_ =>
                               {
                                   _dbContext.Blogs.Remove(_blogToDelete);
                                   _dbContext.SaveChanges();
                               });
    }

    [Test]
    public async Task BlogDeleteTest_MustRemoveFromDb()
    {
        var blogDeleteHandler = new DeleteBlogCommandHandler(_mockLogger.Object, _mockBlogManagerStream.Object, new BlogRepository(_dbContext));
        var deleteBlogCommand = new DeleteBlogCommand(_blogToDelete.Id);

        var result = await blogDeleteHandler.Handle(deleteBlogCommand, new CancellationToken());
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Id.Should().Be(deleteBlogCommand.Id);
        _dbContext.Blogs.FirstOrDefault(b => b.Id == result.Id).Should().BeNull();
    }
}