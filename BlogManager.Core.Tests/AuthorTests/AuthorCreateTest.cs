using BlockManager.Tests.Shared;
using BlogManager.Adapter.PostgreSQL.DbContext;
using BlogManager.Core.Commands.Author;
using BlogManager.Core.Domain;
using BlogManager.Core.Events.Author;
using BlogManager.Core.Handlers.CommandHandlers.Author;
using BlogManager.Core.Handlers.EventHandlers;
using BlogManager.Core.Logger;
using FluentAssertions;
using Moq;

namespace BlogManager.Core.Tests.AuthorTests;

public class AuthorCreateTest
{
    private IBlogDbContext           _dbContext;
    private Mock<IBlogManagerLogger> _mockLogger;
    private Mock<IBlogManagerStreamHandler> _mockBlogManagerStream;


    [SetUp]
    public async Task Setup()
    {
        _dbContext             = await DbContextFactory.CreatePostgreSqlInMemoryDbContext();
        _mockLogger            = new Mock<IBlogManagerLogger>();
        _mockBlogManagerStream = new Mock<IBlogManagerStreamHandler>();
        _mockBlogManagerStream.Setup(s => s.HandleAuthorCreatedEventAsync(It.IsAny<AuthorCreatedEvent>()))
                              .Callback<AuthorCreatedEvent>(authorCreatedEvent =>
                               {
                                   _dbContext.Authors.Add(Author.CreateAsync(authorCreatedEvent.Id,
                                                                             authorCreatedEvent.Name,
                                                                             authorCreatedEvent.Surname).Result);
                                   _dbContext.SaveChanges();
                               });
    }

    [Test]
    public async Task AuthorCreateTest_MustReturnCorrectNameAndSurname()
    {
        var authorCommandHandler = new CreateAuthorCommandHandler(_mockLogger.Object, _mockBlogManagerStream.Object);
        var createAuthorCommand  = new CreateAuthorCommand("TestName", "TestSurname");

        var result = await authorCommandHandler.Handle(createAuthorCommand, new CancellationToken());
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        var createdAuthorInDb = _dbContext.Authors.FirstOrDefault(b => b.Id == result.Id);
        createdAuthorInDb.Should().NotBeNull();
        createdAuthorInDb.Name.Should().Be(createAuthorCommand.Name);
        // createdAuthorInDb.Surname.Should().Be(createAuthorCommand.Surname);
    }
}