using System.Text.Json;
using BlogManager.Adaptor.EventStore.Services;
using BlogManager.Core.Domain;
using BlogManager.Core.DTOs;
using BlogManager.Core.Events.Author;
using BlogManager.Core.Events.Blog;
using BlogManager.Core.Logger;
using BlogManager.Core.Repositories;
using EventStore.ClientAPI;
using Moq;

namespace BlogManager.Adaptor.EventStore.Tests;

public class EventStoreGenerateReadModelServiceTests
{
    private Mock<IEventStoreConnection> _mockEventStoreConnection;
    private Mock<IBlogRepository>       _mockBlogRepository;
    private Mock<IAuthorRepository>     _mockAuthorRepository;
    private Mock<IBlogManagerLogger>    _mockLogger;


    [SetUp]
    public void Setup()
    {
        _mockEventStoreConnection = new Mock<IEventStoreConnection>();
        _mockBlogRepository       = new Mock<IBlogRepository>();
        _mockAuthorRepository     = new Mock<IAuthorRepository>();
        _mockLogger               = new Mock<IBlogManagerLogger>();
    }

    [Test]
    public async Task HandleEvent_BlogCreatedEvent_AddsBlogToRepository()
    {
        
        var service = new EventStoreGenerateReadModelService(
                                                             _mockEventStoreConnection.Object,
                                                             _mockBlogRepository.Object,
                                                             _mockLogger.Object,
                                                             _mockAuthorRepository.Object);

        var blogCreatedEvent = new BlogCreatedEvent
                               {
                                   Id          = Guid.NewGuid(),
                                   AuthorId    = Guid.NewGuid(),
                                   Title       = "Test Blog",
                                   Description = "Test Description",
                                   Content     = "Test Content"
                               };

        var eventData = JsonSerializer.Serialize(blogCreatedEvent);
        var metadata  = blogCreatedEvent.GetType().FullName;

        
        await service.HandleEvent(eventData, metadata);

       
        _mockBlogRepository.Verify(repo => repo.AddBlogAsync(It.IsAny<Blog>()), Times.Once);
    }

    [Test]
    public async Task HandleEvent_BlogUpdatedEvent_UpdatesBlogInRepository()
    {
        
        var service = new EventStoreGenerateReadModelService(
                                                             _mockEventStoreConnection.Object,
                                                             _mockBlogRepository.Object,
                                                             _mockLogger.Object,
                                                             _mockAuthorRepository.Object);

        var blogUpdatedEvent = new BlogUpdatedEvent
                               {
                                   BlogDto = new BlogDto
                                             {
                                                 Id          = Guid.NewGuid(),
                                                 AuthorId    = Guid.NewGuid(),
                                                 Title       = "Test Title",
                                                 Description = "Test Description",
                                                 Content     = "Test Content",
                                                 Author      = null
                                             }
                               };

        var eventData = JsonSerializer.Serialize(blogUpdatedEvent);
        var metadata  = blogUpdatedEvent.GetType().FullName;

        
        await service.HandleEvent(eventData, metadata);

       
        _mockBlogRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Blog>()), Times.Once);
    }

    [Test]
    public async Task HandleEvent_BlogDeletedEvent_DeletesBlogInRepository()
    {
        
        var service = new EventStoreGenerateReadModelService(
                                                             _mockEventStoreConnection.Object,
                                                             _mockBlogRepository.Object,
                                                             _mockLogger.Object,
                                                             _mockAuthorRepository.Object);

        var blogDeletedEvent = new BlogDeletedEvent
                               {
                                   BlogDto = new BlogDto
                                             {
                                                 Id          = Guid.NewGuid(),
                                                 AuthorId    = Guid.NewGuid(),
                                                 Title       = "Test Title",
                                                 Description = "Test Description",
                                                 Content     = "Test Content",
                                                 Author      = null
                                             }
                               };

        var eventData = JsonSerializer.Serialize(blogDeletedEvent);
        var metadata  = blogDeletedEvent.GetType().FullName;

        
        await service.HandleEvent(eventData, metadata);

       
        _mockBlogRepository.Verify(repo => repo.DeleteBlogAsync(It.IsAny<Blog>()), Times.Once);
    }

    [Test]
    public async Task HandleEvent_AuthorCreatedEvent_AddsAuthorToRepository()
    {
        
        var service = new EventStoreGenerateReadModelService(
                                                             _mockEventStoreConnection.Object,
                                                             _mockBlogRepository.Object,
                                                             _mockLogger.Object,
                                                             _mockAuthorRepository.Object);

        var authorCreatedEvent = new AuthorCreatedEvent
                                 {
                                     Id      = Guid.NewGuid(),
                                     Name    = "Test Name",
                                     Surname = "Test Surname"
                                 };

        var eventData = JsonSerializer.Serialize(authorCreatedEvent);
        var metadata  = authorCreatedEvent.GetType().FullName;

        
        await service.HandleEvent(eventData, metadata);

       
        _mockAuthorRepository.Verify(repo => repo.AddAuthorAsync(It.IsAny<Author>()), Times.Once);
    }

    [Test]
    public async Task HandleEvent_AuthorUpdatedEvent_UpdatesAuthorInRepository()
    {
        
        var service = new EventStoreGenerateReadModelService(
                                                             _mockEventStoreConnection.Object,
                                                             _mockBlogRepository.Object,
                                                             _mockLogger.Object,
                                                             _mockAuthorRepository.Object);

        var authorUpdatedEvent = new AuthorUpdatedEvent
                                 {
                                     AuthorDto = new AuthorDto
                                                 {
                                                     Id      = Guid.NewGuid(),
                                                     Name    = "Test Name",
                                                     Surname = "Test Surname"
                                                 }
                                 };

        var eventData = JsonSerializer.Serialize(authorUpdatedEvent);
        var metadata  = authorUpdatedEvent.GetType().FullName;

        
        await service.HandleEvent(eventData, metadata);

       
        _mockAuthorRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Author>()), Times.Once);
    }

    [Test]
    public async Task HandleEvent_AuthorDeletedEvent_DeletesAuthorInRepository()
    {
        
        var service = new EventStoreGenerateReadModelService(
                                                             _mockEventStoreConnection.Object,
                                                             _mockBlogRepository.Object,
                                                             _mockLogger.Object,
                                                             _mockAuthorRepository.Object);

        var authorDeletedEvent = new AuthorDeletedEvent
                                 {
                                     AuthorDto = new AuthorDto
                                                 {
                                                     Id      = Guid.NewGuid(),
                                                     Name    = "Test Name",
                                                     Surname = "Test Surname"
                                                 }
                                 };

        var eventData = JsonSerializer.Serialize(authorDeletedEvent);
        var metadata  = authorDeletedEvent.GetType().FullName;

        
        await service.HandleEvent(eventData, metadata);

       
        _mockAuthorRepository.Verify(repo => repo.DeleteAuthorAsync(It.IsAny<Author>()), Times.Once);
    }
}