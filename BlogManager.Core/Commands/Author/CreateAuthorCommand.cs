using System.Xml.Serialization;
using BlogManager.Core.DTOs;
using MediatR;

namespace BlogManager.Core.Commands.Author;

public class CreateAuthorCommand : IRequest<CreateAuthorResponseDto?>
{
    public CreateAuthorCommand(string name, string surname)
    {
        Name    = name;
        Surname = surname;
    }

    public CreateAuthorCommand()
    {
    }

    [XmlElement("name")]
    public string Name { get; set; }

    [XmlElement("surname")]
    public string Surname { get; set; }
}