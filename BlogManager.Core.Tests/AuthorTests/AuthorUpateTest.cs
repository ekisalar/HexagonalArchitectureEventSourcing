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

public class AuthorUpdateTest
{
    private IBlogDbContext           _dbContext;
    private Mock<IBlogManagerLogger> _mockLogger;
    private Mock<IBlogManagerStreamHandler> _mockBlogManagerStream;
    private Author                   _authorToUpdate;
    private string                   _nameToUpdate;
    private string                   _surnameToUpdate;


    [SetUp]
    public async Task Setup()
    {
        _dbContext             = await DbContextFactory.CreatePostgreSqlInMemoryDbContext();
        _mockLogger            = new Mock<IBlogManagerLogger>();
        _authorToUpdate        = await _dbContext.Authors.FirstAsync();
        _nameToUpdate          = "TestName";
        _surnameToUpdate       = "TestSurname";
        _mockBlogManagerStream = new Mock<IBlogManagerStreamHandler>();
        _mockBlogManagerStream.Setup(s => s.HandleAuthorUpdatedEventAsync(It.IsAny<AuthorUpdatedEvent>()))
                              .Callback<AuthorUpdatedEvent>(_ =>
                               {
                                   _dbContext.Authors.Update(Author.UpdateAsync(_authorToUpdate, _nameToUpdate, _surnameToUpdate).Result);
                                   _dbContext.SaveChanges();
                               });
    }

    [Test]
    public async Task AuthorUpdateTest_MustReturnCorrectIdAndTitle()
    {
        var authorUpdateHandler = new UpdateAuthorCommandHandler(new AuthorRepository(_dbContext), _mockLogger.Object, _mockBlogManagerStream.Object);
        var authorToUpdate      = await _dbContext.Authors.FirstAsync();
        var updateAuthorCommand = new UpdateAuthorCommand(authorToUpdate.Id, "TestName", "TestSurname");

        var result = await authorUpdateHandler.Handle(updateAuthorCommand, new CancellationToken());
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        var updatedAuthorInDb = _dbContext.Authors.FirstOrDefault(b => b.Id == result.Id);
        updatedAuthorInDb.Should().NotBeNull();
        updatedAuthorInDb.Id.Should().Be(updateAuthorCommand.Id);
        updatedAuthorInDb.Name.Should().Be(updateAuthorCommand.Name);
        updatedAuthorInDb.Surname.Should().Be(updateAuthorCommand.Surname);
    }
}