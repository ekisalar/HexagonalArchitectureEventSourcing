using BlogManager.Core.DTOs;
using MediatR;

namespace BlogManager.Core.Events.Blog;

public class BlogDeletedEvent : INotification
{
    public BlogDto BlogDto { get; set; }
}