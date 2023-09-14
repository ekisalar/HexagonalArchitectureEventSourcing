using BlockManager.Tests.Shared;
using BlogManager.Adapter.PostgreSQL.DbContext;
using BlogManager.Adapter.PostgreSQL.Repositories;
using BlogManager.Core.Handlers.QueryHandlers;
using BlogManager.Core.Logger;
using BlogManager.Core.Queries;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BlogManager.Core.Tests.AuthorTests;

public class AuthorGetByIdTest
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
    public async Task AuthorGetByIdTest_MustReturnCorrectAuthor()
    {
        var handler             = new GetAuthorByIdQueryHandler(new AuthorRepository(dbContext), mockLogger.Object);
        var authorFromDbContext = await dbContext.Authors.FirstAsync();

        var result = await handler.Handle(new GetAuthorByIdQuery(authorFromDbContext.Id), CancellationToken.None);
        result.Should().NotBeNull();
        result.Id.Should().Be(authorFromDbContext.Id);
    }
    
}