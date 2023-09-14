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

public class AuthorGetListTest
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
    public async Task AuthorGetListTest_MustReturnCorrectListOfData()
    {
        var handler                 = new GetAuthorListQueryHandler(new AuthorRepository(dbContext), mockLogger.Object);
        var result                  = await handler.Handle(new GetAuthorListQuery(), new CancellationToken());
        var authorListFromDbContext = await dbContext.Authors.ToListAsync();
        result.Should().NotBeNull();
        result.Should().NotBeNull();
        result.Count.Should().Be(authorListFromDbContext.Count);
    }
}