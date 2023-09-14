using BlogManager.Core.DTOs;
using MediatR;

namespace BlogManager.Core.Queries;

public class GetBlogListQuery : IRequest<List<BlogDto>?>
{
    public GetBlogListQuery(bool includeAuthorInfo)
    {
        IncludeAuthorInfo = includeAuthorInfo;
    }

    public bool IncludeAuthorInfo { get; set; }
}