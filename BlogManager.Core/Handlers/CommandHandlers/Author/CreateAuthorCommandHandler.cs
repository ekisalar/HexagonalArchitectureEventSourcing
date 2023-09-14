using BlogManager.Core.Commands.Author;
using BlogManager.Core.DTOs;
using BlogManager.Core.Events.Author;
using BlogManager.Core.Handlers.EventHandlers;
using BlogManager.Core.Logger;
using Mapster;
using MediatR;

namespace BlogManager.Core.Handlers.CommandHandlers.Author;

public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, CreateAuthorResponseDto?>
{
    private readonly IBlogManagerLogger _logger;
    private readonly IBlogManagerStreamHandler _blogManagerStreamHandler;

    public CreateAuthorCommandHandler(IBlogManagerLogger logger, IBlogManagerStreamHandler blogManagerStreamHandler)
    {
        _logger            = logger;
        _blogManagerStreamHandler = blogManagerStreamHandler;
    }

    public async Task<CreateAuthorResponseDto?> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        var authorToCreate     = await Domain.Author.CreateAsync(Guid.NewGuid(), request.Name, request.Surname);
        var authorCreatedEvent = authorToCreate.Adapt<AuthorCreatedEvent>();
        await _blogManagerStreamHandler.HandleAuthorCreatedEventAsync(authorCreatedEvent);
        _logger.LogInformation($"Author with ID {authorToCreate.Id} create event successfully sent to queue.");
        return new CreateAuthorResponseDto() {Id = authorToCreate.Id};
    }
}