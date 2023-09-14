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
using Moq;

namespace BlogManager.Core.Tests.BlogTests;

public class BlogCreateTest
{
    private IBlogDbContext                  _dbContext;
    private Mock<IBlogManagerLogger>        _mockLogger;
    private Mock<IBlogManagerStreamHandler> _mockBlogManagerStream;


    [SetUp]
    public async Task Setup()
    {
        _dbContext             = await DbContextFactory.CreatePostgreSqlInMemoryDbContext();
        _mockLogger            = new Mock<IBlogManagerLogger>();
        _mockBlogManagerStream = new Mock<IBlogManagerStreamHandler>();

        _mockBlogManagerStream.Setup(s => s.HandleBlogCreatedEventAsync(It.IsAny<BlogCreatedEvent>()))
                              .Callback<BlogCreatedEvent>(blogCreatedEvent =>
                               {
                                   _dbContext.Blogs.Add(Blog.CreateAsync(blogCreatedEvent.Id,
                                                                         blogCreatedEvent.AuthorId,
                                                                         blogCreatedEvent.Title,
                                                                         blogCreatedEvent.Description,
                                                                         blogCreatedEvent.Content).Result);
                                   _dbContext.SaveChanges();
                               });
    }

    [Test]
    public async Task BlogCreateTest_MustReturnCorrectAuthorId()
    {
        var blogCommandHandler = new CreateBlogCommandHandler(_mockLogger.Object, _mockBlogManagerStream.Object, new AuthorRepository(_dbContext));
        var createBlogCommand  = new CreateBlogCommand(_dbContext.Authors.First().Id, "Test Title", "Test Description", "Test Content");
        var result             = await blogCommandHandler.Handle(createBlogCommand, new CancellationToken());
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        var createdBlogInDb = _dbContext.Blogs.FirstOrDefault(b => b.Id == result.Id);
        createdBlogInDb.Should().NotBeNull();
        createdBlogInDb.AuthorId.Should().Be(createBlogCommand.AuthorId);
    }
}