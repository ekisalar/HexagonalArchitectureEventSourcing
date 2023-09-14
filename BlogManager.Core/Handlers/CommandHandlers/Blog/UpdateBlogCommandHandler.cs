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

public class UpdateBlogCommandHandler : IRequestHandler<UpdateBlogCommand, UpdateBlogResponseDto?>
{
    private readonly IBlogRepository    _blogRepository;
    private readonly IBlogManagerLogger _logger;
    private readonly IBlogManagerStreamHandler        _blogManagerStreamHandler;

    public UpdateBlogCommandHandler(IBlogRepository blogRepository, IBlogManagerLogger logger, IBlogManagerStreamHandler blogManagerStreamHandler)
    {
        _blogRepository = blogRepository;
        _logger         = logger;
        _blogManagerStreamHandler     = blogManagerStreamHandler;
    }


    public async Task<UpdateBlogResponseDto?> Handle(UpdateBlogCommand request, CancellationToken cancellationToken)
    {
        var blogToUpdate = await _blogRepository.GetBlogByIdAsync(request.Id, false, false);
        if (blogToUpdate is null)
            throw new Exception(ExceptionConstants.BlogNotFound);
        await Domain.Blog.UpdateAsync(blogToUpdate, request.AuthorId, request.Title, request.Description, request.Content);
        var blogUpdatedEvent = new BlogUpdatedEvent() {BlogDto = blogToUpdate.Adapt<BlogDto>()};
        await _blogManagerStreamHandler.HandleBlogUpdatedEventAsync(blogUpdatedEvent);
        _logger.LogInformation($"Blog with ID {request.Id} updated event successfully sent to queue.");
        return blogUpdatedEvent.BlogDto.Adapt<UpdateBlogResponseDto>();
    }
}