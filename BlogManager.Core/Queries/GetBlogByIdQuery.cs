using BlogManager.Core.DTOs;
using MediatR;

namespace BlogManager.Core.Queries;

public class GetBlogByIdQuery : IRequest<BlogDto?>
{
    public GetBlogByIdQuery(Guid id, bool includeAuthorInfo = false)
    {
        Id                = id;
        IncludeAuthorInfo = includeAuthorInfo;
    }

    public Guid Id                { get; set; }
    public bool IncludeAuthorInfo { get; set; }
}