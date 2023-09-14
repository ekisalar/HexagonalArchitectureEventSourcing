using BlogManager.Core.DTOs;
using MediatR;

namespace BlogManager.Core.Queries;

public class GetAuthorByIdQuery : IRequest<AuthorDto?>
{
    public GetAuthorByIdQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}