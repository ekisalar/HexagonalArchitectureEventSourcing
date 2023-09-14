using BlogManager.Core.Events.Author;
using BlogManager.Core.Events.Blog;

namespace BlogManager.Core.Handlers.EventHandlers;

public interface IBlogManagerStreamHandler
{
    Task HandleBlogCreatedEventAsync(BlogCreatedEvent     blogCreatedEvent);
    Task HandleBlogUpdatedEventAsync(BlogUpdatedEvent     blogUpdatedEvent);
    Task HandleBlogDeletedEventAsync(BlogDeletedEvent     blogDeletedEvent);
    Task HandleAuthorCreatedEventAsync(AuthorCreatedEvent authorCreatedEvent);
    Task HandleAuthorUpdatedEventAsync(AuthorUpdatedEvent authorUpdatedEvent);
    Task HandleAuthorDeletedEventAsync(AuthorDeletedEvent authorDeletedEvent);
}