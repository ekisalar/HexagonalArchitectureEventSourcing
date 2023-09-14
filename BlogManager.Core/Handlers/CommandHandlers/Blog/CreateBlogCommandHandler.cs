using BlogManager.Core.Commands.Blog;
using BlogManager.Core.Constants;
using BlogManager.Core.DTOs;
using BlogManager.Core.Events.Blog;
using BlogManager.Core.Handlers.EventHandlers;
using BlogManager.Core.Logger;
using BlogManager.Core.Repositories;
using Mapster;
using MediatR;

namespace BlogManager.Core.Handlers.CommandHandlers.Blog;

public class CreateBlogCommandHandler : IRequestHandler<CreateBlogCommand, CreateBlogResponseDto?>
{
    private readonly IBlogManagerStreamHandler _blogManagerStreamHandler;
    private readonly IBlogManagerLogger        _logger;
    private readonly IAuthorRepository         _authorRepository;

    public CreateBlogCommandHandler(IBlogManagerLogger logger, IBlogManagerStreamHandler blogManagerStreamHandler, IAuthorRepository authorRepository)
    {
        _logger                   = logger;
        _blogManagerStreamHandler = blogManagerStreamHandler;
        _authorRepository         = authorRepository;
    }

    public async Task<CreateBlogResponseDto?> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
    {
        var author = await _authorRepository.GetAuthorByIdAsync(request.AuthorId);
        if (author == null)
            throw new Exception(ExceptionConstants.AuthorNotFound);
        var blogToCreate     = await Domain.Blog.CreateAsync(Guid.NewGuid(), request.AuthorId, request.Title, request.Description, request.Content);
        var blogCreatedEvent = blogToCreate.Adapt<BlogCreatedEvent>();
        await _blogManagerStreamHandler.HandleBlogCreatedEventAsync(blogCreatedEvent);
        _logger.LogInformation($"Blog with ID {blogCreatedEvent.Id} created event successfully sent to queue.");
        return new CreateBlogResponseDto() {Id = blogCreatedEvent.Id};
    }
}