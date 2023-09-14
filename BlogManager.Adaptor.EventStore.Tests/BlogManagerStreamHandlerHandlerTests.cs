using BlogManager.Adaptor.EventStore.Services;
using BlogManager.Core.DTOs;
using BlogManager.Core.Events.Author;
using BlogManager.Core.Events.Blog;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using FluentAssertions;
using Moq;

namespace BlogManager.Adaptor.EventStore.Tests
{
    public class BlogManagerStreamHandlerHandlerTests
    {
        private Mock<IEventStoreConnection> _eventStoreConnectionMock;
        private BlogManagerStreamHandlerHandler _blogManagerStreamHandlerHandler;
        private const string StreamName = "BlogStream";

        [SetUp]
        public void Setup()
        {
            _eventStoreConnectionMock = new Mock<IEventStoreConnection>(); // Initialize the mock
            _eventStoreConnectionMock
                .Setup(x => x.AppendToStreamAsync(StreamName, ExpectedVersion.Any, It.IsAny<IEnumerable<EventData>>(), It.IsAny<UserCredentials>()))
                .Returns(Task.FromResult(new WriteResult(ExpectedVersion.Any, Position.Start)));
            _blogManagerStreamHandlerHandler = new BlogManagerStreamHandlerHandler(_eventStoreConnectionMock.Object);
        }

        
         [Test]
        public async Task HandleBlogCreatedEventAsync_ShouldAddEventAndSaveAsync()
        {
            
            var blogCreatedEvent = new BlogCreatedEvent
                                   {
                                       Id          = Guid.NewGuid(),
                                       AuthorId    = Guid.NewGuid(),
                                       Title       = "Test Title",
                                       Description = "Test Description",
                                       Content     = "Test Content"
                                   };

            
            await _blogManagerStreamHandlerHandler.HandleBlogCreatedEventAsync(blogCreatedEvent);

           
            _blogManagerStreamHandlerHandler.Events.Should().BeEmpty();
            _eventStoreConnectionMock.Verify(x => x.AppendToStreamAsync(StreamName, ExpectedVersion.Any, It.IsAny<IEnumerable<EventData>>(), It.IsAny<UserCredentials>()), Times.Once);
        }
        
        [Test]
        public async Task HandleBlogUpdatedEventAsync_ShouldAddEventAndSaveAsync()
        {
            
            var blogUpdatedEvent = new BlogUpdatedEvent()
                                     {
                                         BlogDto = new BlogDto
                                                     {
                                                         Id          = Guid.NewGuid(),
                                                         AuthorId    = Guid.NewGuid(),
                                                         Title       = "Test Title",
                                                         Description = "Test Description",
                                                         Content     = "Test Content"
                                                     }
                                     };

            
            await _blogManagerStreamHandlerHandler.HandleBlogUpdatedEventAsync(blogUpdatedEvent);

           
            _blogManagerStreamHandlerHandler.Events.Should().BeEmpty();
            _eventStoreConnectionMock.Verify(x => x.AppendToStreamAsync(StreamName, ExpectedVersion.Any, It.IsAny<IEnumerable<EventData>>(), It.IsAny<UserCredentials>()), Times.Once);
        }
        
        [Test]
        public async Task HandleBlogDeletedEventAsync_ShouldAddEventAndSaveAsync()
        {
            
            var blogDeletedEvent = new BlogDeletedEvent
            {
                BlogDto = new BlogDto
                {
                    Id          = Guid.NewGuid(),
                    AuthorId    = Guid.NewGuid(),
                    Title       = "Test Title",
                    Description = "Test Description",
                    Content     = "Test Content"
                }
            };

            
            await _blogManagerStreamHandlerHandler.HandleBlogDeletedEventAsync(blogDeletedEvent);

           
            _blogManagerStreamHandlerHandler.Events.Should().BeEmpty();
            _eventStoreConnectionMock.Verify(x => x.AppendToStreamAsync(StreamName, ExpectedVersion.Any, It.IsAny<IEnumerable<EventData>>(), It.IsAny<UserCredentials>()), Times.Once);
        }
        
        [Test]
        public async Task HandleAuthorCreatedEventAsync_ShouldAddEventAndSaveAsync()
        {
            
            var authorCreatedEvent = new AuthorCreatedEvent()
                                     {
                                         Id      = Guid.NewGuid(),
                                         Name    = "Test Name",
                                         Surname = "Test Surname"
                                     };

            
            await _blogManagerStreamHandlerHandler.HandleAuthorCreatedEventAsync(authorCreatedEvent);

           
            _blogManagerStreamHandlerHandler.Events.Should().BeEmpty();
            _eventStoreConnectionMock.Verify(x => x.AppendToStreamAsync(StreamName, ExpectedVersion.Any, It.IsAny<IEnumerable<EventData>>(), It.IsAny<UserCredentials>()), Times.Once);
        }
        
        [Test]
        public async Task HandleAuthorUpdatedEventAsync_ShouldAddEventAndSaveAsync()
        {
            
            var authorUpdatedEvent = new AuthorUpdatedEvent()
                                     {
                                         AuthorDto = new AuthorDto
                                                     {
                                                         Id      = Guid.NewGuid(),
                                                         Name    = "Test Name",
                                                         Surname = "Test Surname"
                                                     }
                                     };

            
            await _blogManagerStreamHandlerHandler.HandleAuthorUpdatedEventAsync(authorUpdatedEvent);

           
            _blogManagerStreamHandlerHandler.Events.Should().BeEmpty();
            _eventStoreConnectionMock.Verify(x => x.AppendToStreamAsync(StreamName, ExpectedVersion.Any, It.IsAny<IEnumerable<EventData>>(), It.IsAny<UserCredentials>()), Times.Once);
        }
        
        [Test]
        public async Task HandleAuthorDeletedEventAsync_ShouldAddEventAndSaveAsync()
        {
            
            var authorDeletedEvent = new AuthorDeletedEvent
            {
                AuthorDto = new AuthorDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Name",
                    Surname = "Test Surname"
                }
            };

            
            await _blogManagerStreamHandlerHandler.HandleAuthorDeletedEventAsync(authorDeletedEvent);

           
            _blogManagerStreamHandlerHandler.Events.Should().BeEmpty();
            _eventStoreConnectionMock.Verify(x => x.AppendToStreamAsync(StreamName, ExpectedVersion.Any, It.IsAny<IEnumerable<EventData>>(), It.IsAny<UserCredentials>()), Times.Once);
        }
    }
}
