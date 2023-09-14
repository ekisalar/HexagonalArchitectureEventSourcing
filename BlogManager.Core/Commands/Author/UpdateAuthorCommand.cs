using System.Xml.Serialization;
using BlogManager.Core.DTOs;
using MediatR;

namespace BlogManager.Core.Commands.Author;

public class UpdateAuthorCommand : IRequest<UpdateAuthorResponseDto>
{
    public UpdateAuthorCommand(Guid id, string name, string surname)
    {
        Id      = id;
        Name    = name;
        Surname = surname;
    }

    public UpdateAuthorCommand()
    {
    }
    
    [XmlElement("id")]
    public Guid   Id      { get; }
    
    [XmlElement("name")]
    public string Name    { get; }
    
    [XmlElement("surname")]
    public string Surname { get; }
}