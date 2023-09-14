using BlogManager.Core.Commands.Author;
using BlogManager.Core.Constants;
using BlogManager.Core.DTOs;
using BlogManager.Core.Events.Author;
using BlogManager.Core.Handlers.EventHandlers;
using BlogManager.Core.Logger;
using BlogManager.Core.Repositories;
using Mapster;
using MediatR;

namespace BlogManager.Core.Handlers.CommandHandlers.Author;

public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand, UpdateAuthorResponseDto?>
{
    private readonly IAuthorRepository  _authorRepository;
    private readonly IBlogManagerLogger _logger;
    private readonly IBlogManagerStreamHandler _blogManagerStreamHandler;

    public UpdateAuthorCommandHandler(IAuthorRepository authorRepository, IBlogManagerLogger logger, IBlogManagerStreamHandler blogManagerStreamHandler)
    {
        _authorRepository  = authorRepository;
        _logger            = logger;
        _blogManagerStreamHandler = blogManagerStreamHandler;
    }


    public async Task<UpdateAuthorResponseDto?> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        var authorToUpdate = await _authorRepository.GetAuthorByIdAsync(request.Id, false);
        if (authorToUpdate is null)
            throw new Exception(ExceptionConstants.AuthorNotFound);
        await Domain.Author.UpdateAsync(authorToUpdate, request.Name, request.Surname);
        var authorUpdatedEvent = new AuthorUpdatedEvent() {AuthorDto = authorToUpdate.Adapt<AuthorDto>()};
        await _blogManagerStreamHandler.HandleAuthorUpdatedEventAsync(authorUpdatedEvent);
        _logger.LogInformation($"Author with ID {request.Id} updated event successfully sent to queue.");
        return authorUpdatedEvent.AuthorDto.Adapt<UpdateAuthorResponseDto>();
    }
}