using MediatR;

namespace BlogManager.Core.Events.Author;

public class AuthorCreatedEvent : INotification
{
    public Guid   Id      { get; set; }
    public string Name    { get; set; }
    public string Surname { get; set; }
}