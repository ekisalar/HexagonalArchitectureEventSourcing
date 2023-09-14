using BlogManager.Core.DTOs;
using MediatR;

namespace BlogManager.Core.Queries;

public class GetAuthorListQuery : IRequest<List<AuthorDto>?>
{
}