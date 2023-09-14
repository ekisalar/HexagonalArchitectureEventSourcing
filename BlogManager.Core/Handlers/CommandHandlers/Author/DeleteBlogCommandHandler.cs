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

public class DeleteAuthorCommandHandler : IRequestHandler<DeleteAuthorCommand, DeleteAuthorResponseDto>
{
    private readonly IAuthorRepository  _authorRepository;
    private readonly IBlogManagerLogger _blogManagerLogger;
    private readonly IBlogManagerStreamHandler _blogManagerStreamHandler;

    public DeleteAuthorCommandHandler(IAuthorRepository authorRepository, IBlogManagerLogger blogManagerLogger, IBlogManagerStreamHandler blogManagerStreamHandler)
    {
        _authorRepository  = authorRepository;
        _blogManagerLogger = blogManagerLogger;
        _blogManagerStreamHandler = blogManagerStreamHandler;
    }


    public async Task<DeleteAuthorResponseDto> Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
    {
        var authorToDelete = await _authorRepository.GetAuthorByIdAsync(request.Id, false);
        if (authorToDelete is null)
            throw new Exception(ExceptionConstants.AuthorNotFound);
        await Domain.Author.DeleteAsync(authorToDelete);
        var authorDeletedEvent = new AuthorDeletedEvent(){AuthorDto = authorToDelete.Adapt<AuthorDto>()};
       await _blogManagerStreamHandler.HandleAuthorDeletedEventAsync(authorDeletedEvent);
        _blogManagerLogger.LogInformation($"Author with ID {request.Id}  deleted event successfully sent to queue.");
        return new DeleteAuthorResponseDto() {Id = authorToDelete.Id};
    }
}