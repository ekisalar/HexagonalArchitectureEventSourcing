using BlockManager.Tests.Shared;
using BlogManager.Adapter.PostgreSQL.DbContext;
using BlogManager.Adapter.PostgreSQL.Repositories;
using BlogManager.Core.Commands.Author;
using BlogManager.Core.Domain;
using BlogManager.Core.Events.Author;
using BlogManager.Core.Handlers.CommandHandlers.Author;
using BlogManager.Core.Handlers.EventHandlers;
using BlogManager.Core.Logger;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BlogManager.Core.Tests.AuthorTests;

public class AuthorDeleteTest
{
    private IBlogDbContext           _dbContext;
    private Mock<IBlogManagerLogger> _mockLogger;
    private Mock<IBlogManagerStreamHandler> _mockAuthorManagerStream;
    private Author                     _authorToDelete;


    [SetUp]
    public async Task Setup()
    {
        _dbContext              = await DbContextFactory.CreatePostgreSqlInMemoryDbContext();
        _mockLogger             = new Mock<IBlogManagerLogger>();
        _authorToDelete          = await _dbContext.Authors.FirstAsync();
        _mockAuthorManagerStream = new Mock<IBlogManagerStreamHandler>();
        _mockAuthorManagerStream.Setup(s => s.HandleAuthorDeletedEventAsync(It.IsAny<AuthorDeletedEvent>()))
                              .Callback<AuthorDeletedEvent>(_ =>
                               {
                                   _dbContext.Authors.Remove(_authorToDelete);
                                   _dbContext.SaveChanges();
                               });

    }

    [Test]
    public async Task AuthorDeleteTest_MustRemoveFromDb()
    {
        var authorDeleteHandler = new DeleteAuthorCommandHandler(new AuthorRepository(_dbContext), _mockLogger.Object, _mockAuthorManagerStream.Object);
        var deleteAuthorCommand = new DeleteAuthorCommand(_authorToDelete.Id);
        var result = await authorDeleteHandler.Handle(deleteAuthorCommand, new CancellationToken());
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Id.Should().Be(deleteAuthorCommand.Id);
        _dbContext.Authors.FirstOrDefault(b => b.Id == result.Id).Should().BeNull();
    }
}