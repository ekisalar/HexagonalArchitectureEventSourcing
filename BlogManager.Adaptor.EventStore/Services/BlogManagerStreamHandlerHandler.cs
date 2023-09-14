using BlogManager.Adaptor.EventStore.Infrastructure;
using BlogManager.Core.Events.Author;
using BlogManager.Core.Events.Blog;
using BlogManager.Core.Handlers.EventHandlers;
using EventStore.ClientAPI;

namespace BlogManager.Adaptor.EventStore.Services
{
    public class BlogManagerStreamHandlerHandler : AbstractStreamHandler, IBlogManagerStreamHandler
    {
        private static string StreamName => "BlogStream";


        public BlogManagerStreamHandlerHandler(IEventStoreConnection eventStoreConnection) : base(StreamName, eventStoreConnection)
        {
        }

        public async Task HandleBlogCreatedEventAsync(BlogCreatedEvent blogCreatedEvent)
        {
            Events.AddLast(blogCreatedEvent);
            await SaveAsync();
        }

        public async Task HandleBlogUpdatedEventAsync(BlogUpdatedEvent blogUpdatedEvent)
        {
            Events.AddLast(blogUpdatedEvent);
            await SaveAsync();
        }

        public async Task HandleBlogDeletedEventAsync(BlogDeletedEvent blogDeletedEvent)
        {
            Events.AddLast(blogDeletedEvent);
            await SaveAsync();
        }
        
        public async Task HandleAuthorCreatedEventAsync(AuthorCreatedEvent authorCreatedEvent)
        {
            Events.AddLast(authorCreatedEvent);
            await SaveAsync();
        }

        public async Task HandleAuthorUpdatedEventAsync(AuthorUpdatedEvent authorUpdatedEvent)
        {
            Events.AddLast(authorUpdatedEvent);
            await SaveAsync();
        }
        
        public async Task HandleAuthorDeletedEventAsync(AuthorDeletedEvent authorDeletedEvent)
        {
            Events.AddLast(authorDeletedEvent);
            await SaveAsync();
        }
    }
}