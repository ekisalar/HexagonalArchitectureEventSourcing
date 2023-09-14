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

public class DeleteBlogCommandHandler : IRequestHandler<DeleteBlogCommand, DeleteBlogResponseDto>
{
    private readonly IBlogManagerLogger _blogManagerLogger;
    private readonly IBlogManagerStreamHandler        _blogManagerStreamHandler;
    private readonly IBlogRepository    _blogRepository;

    public DeleteBlogCommandHandler(IBlogManagerLogger blogManagerLogger, IBlogManagerStreamHandler blogManagerStreamHandler, IBlogRepository blogRepository)
    {
        _blogManagerLogger = blogManagerLogger;
        _blogManagerStreamHandler        = blogManagerStreamHandler;
        _blogRepository    = blogRepository;
    }


    public async Task<DeleteBlogResponseDto> Handle(DeleteBlogCommand request, CancellationToken cancellationToken)
    {
        var blogToDelete = await _blogRepository.GetBlogByIdAsync(request.Id, false, false);
        if (blogToDelete is null)
            throw new Exception(ExceptionConstants.BlogNotFound);
        await Domain.Blog.DeleteAsync(blogToDelete);
        var blogDeletedEvent = new BlogDeletedEvent() {BlogDto = blogToDelete.Adapt<BlogDto>()};
        await _blogManagerStreamHandler.HandleBlogDeletedEventAsync(blogDeletedEvent);
        _blogManagerLogger.LogInformation($"Blog with ID {request.Id} deleted event successfully sent to queue.");
        return new DeleteBlogResponseDto() {Id = blogToDelete.Id};
    }
}