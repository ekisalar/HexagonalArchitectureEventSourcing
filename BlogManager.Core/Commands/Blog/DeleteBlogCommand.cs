using System.Xml.Serialization;
using BlogManager.Core.DTOs;
using MediatR;

namespace BlogManager.Core.Commands.Blog;

public class DeleteBlogCommand : IRequest<DeleteBlogResponseDto>
{
   public DeleteBlogCommand(Guid id)
   {
      Id = id;
   }

   public DeleteBlogCommand()
   {
      
   }
   
   [XmlElement("id")]
   public Guid Id { get; }
}