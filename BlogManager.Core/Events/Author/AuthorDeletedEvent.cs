using BlogManager.Core.DTOs;
using MediatR;

namespace BlogManager.Core.Events.Author;

public class AuthorDeletedEvent : INotification
{
    public AuthorDto AuthorDto { get; set; }
}