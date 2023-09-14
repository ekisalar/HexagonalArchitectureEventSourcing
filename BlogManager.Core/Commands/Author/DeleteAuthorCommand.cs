using System.Xml.Serialization;
using BlogManager.Core.DTOs;
using MediatR;

namespace BlogManager.Core.Commands.Author;

public class DeleteAuthorCommand : IRequest<DeleteAuthorResponseDto>
{
   public DeleteAuthorCommand(Guid id)
   {
      Id = id;
   }

   public DeleteAuthorCommand()
   {
      
   }

   [XmlElement("id")]
   public Guid Id { get;  }
}