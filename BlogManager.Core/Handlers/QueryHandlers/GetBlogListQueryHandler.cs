using BlogManager.Core.DTOs;
using BlogManager.Core.Logger;
using BlogManager.Core.Queries;
using BlogManager.Core.Repositories;
using Mapster;
using MediatR;

namespace BlogManager.Core.Handlers.QueryHandlers;

public class GetBlogListQueryHandler : IRequestHandler<GetBlogListQuery, List<BlogDto>?>
{
    private readonly IBlogRepository    _blogRepository;
    private readonly IBlogManagerLogger _logger;

    public GetBlogListQueryHandler(IBlogRepository blogRepository, IBlogManagerLogger logger)
    {
        _blogRepository = blogRepository;
        _logger         = logger;
    }

    public async Task<List<BlogDto>?> Handle(GetBlogListQuery request, CancellationToken cancellationToken)
    {
        var blogs = await _blogRepository.GetAllBlogsAsync(request.IncludeAuthorInfo);
        if (blogs == null)
        {
            _logger.LogInformation("No Blogs found.");
            return null;
        }
        _logger.LogInformation($"{blogs.Count} Blogs retrieved successfully.");
        return blogs?.Adapt<List<BlogDto>>();
    }
}