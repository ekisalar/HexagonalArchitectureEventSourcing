using BlogManager.Core.DTOs;
using BlogManager.Core.Logger;
using BlogManager.Core.Queries;
using BlogManager.Core.Repositories;
using Mapster;
using MediatR;

namespace BlogManager.Core.Handlers.QueryHandlers;

public class GetAuthorByIdQueryHandler : IRequestHandler<GetAuthorByIdQuery, AuthorDto?>
{
    private readonly IAuthorRepository  _authorRepository;
    private          IBlogManagerLogger _logger;

    public GetAuthorByIdQueryHandler(IAuthorRepository authorRepository, IBlogManagerLogger logger)
    {
        _authorRepository = authorRepository;
        _logger           = logger;
    }


    public async Task<AuthorDto?> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
    {
        var author = await _authorRepository.GetAuthorByIdAsync(request.Id);
        if (author == null)
        {
            _logger.LogInformation($"Author with ID {request.Id} not found.");
            return null;
        }

        _logger.LogInformation($"Author with ID {request.Id} retrieved successfully.");
        return author?.Adapt<AuthorDto>();
    }
}