using MediatR;

namespace BlogManager.Core.Events.Blog;

public class BlogCreatedEvent : INotification
{
    public Guid   Id          { get; set; }
    public Guid   AuthorId    { get; set; }
    public string Title       { get; set; }
    public string Description { get; set; }
    public string Content     { get; set; }
}