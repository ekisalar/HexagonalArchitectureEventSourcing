using BlogManager.Core.Commands.Blog;
using BlogManager.Core.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogManager.Adapter.Api.Controllers;

[Route("[controller]/[action]")]
[Consumes("application/json", "application/xml")]
[Produces("application/json", "application/xml")]
[ApiController]
public class BlogController :ControllerBase
{
    private readonly IMediator _mediator;

    public BlogController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBlog([FromBody]CreateBlogCommand createBlogCommand)
    {
        var result = await _mediator.Send(createBlogCommand);
        if (result != null)
            return Ok(result);

        return BadRequest("Failed To Create The Blog");
    }

    [HttpGet]
    public async Task<IActionResult> GetBlog([FromQuery]string id, [FromQuery]bool authorInfo)
    {
        var getBlogByIdQuery = new GetBlogByIdQuery(Guid.Parse(id), authorInfo);
        var result           = await _mediator.Send(getBlogByIdQuery);
        if (result != null)
            return Ok(result);

        return BadRequest("Failed To Get The Blog");
    }

    [HttpGet]
    public async Task<IActionResult> GetBlogList([FromQuery]bool authorInfo)
    {
        var getBlogListQuery = new GetBlogListQuery(authorInfo);
        var result           = await _mediator.Send(getBlogListQuery);
        if (result != null)
            return Ok(result);

        return BadRequest("Failed To Get The Blog List");
    }
}