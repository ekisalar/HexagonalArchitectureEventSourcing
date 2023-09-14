using System.Xml.Serialization;
using BlogManager.Core.DTOs;
using MediatR;

namespace BlogManager.Core.Commands.Blog;

public class CreateBlogCommand : IRequest<CreateBlogResponseDto?>
{
    public CreateBlogCommand(Guid authorId, string title, string description, string content)
    {
        AuthorId    = authorId;
        Title       = title;
        Description = description;
        Content     = content;
    }

    public CreateBlogCommand()
    {
        
    }

    [XmlElement("authorid")]
    public Guid AuthorId { get; set; }

    [XmlElement("title")]
    public string Title { get; set; }

    [XmlElement("description")]
    public string Description { get; set; }

    [XmlElement("content")]
    public string Content { get; set; }
}