using BlogManager.Core.Commands.Author;
using BlogManager.Core.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogManager.Adapter.Api.Controllers;

[ApiController]
[Consumes("application/json", "application/xml")]
[Produces("application/json", "application/xml")]
[Route("[controller]/[action]")]
public class AuthorController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthorController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAuthor([FromBody]CreateAuthorCommand createAuthorCommand)
    {
        var result = await _mediator.Send(createAuthorCommand);
        if (result != null)
            return Ok(result);

        return BadRequest("Failed To Create The Author");
    }

    [HttpGet]
    public async Task<IActionResult> GetAuthor([FromQuery]Guid id)
    {
        var getBlogByIdQuery = new GetAuthorByIdQuery(id);
        var result           = await _mediator.Send(getBlogByIdQuery);
        if (result != null)
            return Ok(result);

        return BadRequest("Failed To Get The Author");
    }

    [HttpGet]
    public async Task<IActionResult> GetAuthorList()
    {
        var getBlogByIdQuery = new GetAuthorListQuery();
        var result           = await _mediator.Send(getBlogByIdQuery);
        if (result != null)
            return Ok(result);

        return BadRequest("Failed To Get The Author List");
    }
}