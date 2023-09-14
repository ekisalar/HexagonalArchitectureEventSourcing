using System.Xml.Serialization;
using BlogManager.Core.DTOs;
using MediatR;

namespace BlogManager.Core.Commands.Blog;

public class UpdateBlogCommand : IRequest<UpdateBlogResponseDto>
{
    public UpdateBlogCommand(Guid id, Guid authorId, string title, string description, string content)
    {
        Id          = id;
        AuthorId    = authorId;
        Title       = title;
        Description = description;
        Content     = content;
    }

    public UpdateBlogCommand()
    {
    }

    [XmlElement("id")]
    public Guid Id { get; }

    [XmlElement("authorid")]
    public Guid AuthorId { get; }

    [XmlElement("title")]
    public string Title { get; }

    [XmlElement("description")]
    public string Description { get; }

    [XmlElement("content")]
    public string Content { get; }
}