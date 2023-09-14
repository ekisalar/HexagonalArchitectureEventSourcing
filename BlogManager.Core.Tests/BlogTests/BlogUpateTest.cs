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

public class BlogUpdateTest
{
    private IBlogDbContext           _dbContext;
    private Mock<IBlogManagerLogger> _mockLogger;
    private Mock<IBlogManagerStreamHandler> _mockBlogManagerStream;
    private Blog                     _blogToUpdate;
    private Guid                     _authorIdToUpdate;
    private string                   _titleToUpdate;
    private string                   _descriptionToUpdate;
    private string                   _contentToUpdate;


    [SetUp]
    public async Task Setup()
    {
        _dbContext             = await DbContextFactory.CreatePostgreSqlInMemoryDbContext();
        _mockLogger            = new Mock<IBlogManagerLogger>();
        _blogToUpdate          = await _dbContext.Blogs.FirstAsync();
        _authorIdToUpdate      = Guid.NewGuid();
        _titleToUpdate         = "Test Title Updated";
        _descriptionToUpdate   = "Test Description Updated";
        _contentToUpdate       = "Test Content Updated";
        _mockBlogManagerStream = new Mock<IBlogManagerStreamHandler>();
        _mockBlogManagerStream.Setup(s => s.HandleBlogUpdatedEventAsync(It.IsAny<BlogUpdatedEvent>()))
                              .Callback<BlogUpdatedEvent>(_ =>
                               {
                                   _dbContext.Blogs.Update(Blog.UpdateAsync(_blogToUpdate, _authorIdToUpdate, _titleToUpdate, _descriptionToUpdate, _contentToUpdate).Result);
                                   _dbContext.SaveChanges();
                               });
    }

    [Test]
    public async Task BlogUpdateTest_MustReturnCorrectIdAndTitle()
    {
        var blogUpdateHandler = new UpdateBlogCommandHandler(new BlogRepository(_dbContext), _mockLogger.Object, _mockBlogManagerStream.Object);
        var updateBlogCommand = new UpdateBlogCommand(_blogToUpdate.Id, _authorIdToUpdate, _titleToUpdate, _descriptionToUpdate, _contentToUpdate);

        var result = await blogUpdateHandler.Handle(updateBlogCommand, new CancellationToken());
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        var updatedBlogInDb = _dbContext.Blogs.FirstOrDefault(b => b.Id == result.Id);
        updatedBlogInDb.Should().NotBeNull();
        updatedBlogInDb.AuthorId.Should().Be(updateBlogCommand.AuthorId);
        updatedBlogInDb.Title.Should().Be(updateBlogCommand.Title);
        updatedBlogInDb.Description.Should().Be(updateBlogCommand.Description);
        updatedBlogInDb.Content.Should().Be(updateBlogCommand.Content);
    }
}