using BlogManager.Core.DTOs;
using MediatR;

namespace BlogManager.Core.Events.Blog;

public class BlogUpdatedEvent : INotification
{
    public BlogDto BlogDto { get; set; }
}