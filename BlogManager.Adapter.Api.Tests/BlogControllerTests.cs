using BlogManager.Adapter.Api.Controllers;
using BlogManager.Core.Commands.Blog;
using BlogManager.Core.DTOs;
using BlogManager.Core.Queries;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BlogManager.Adapter.Api.Tests
{
    public class BlogControllerTests
    {
        [Test]
        public async Task CreateBlog_ReturnsOkResult()
        {
            
            var mediatorMock      = new Mock<IMediator>();
            var createBlogCommand = new CreateBlogCommand(Guid.NewGuid(), "TestTitle", "Test Description", "TestContent");
            var expectedResult    = new CreateBlogResponseDto();

            mediatorMock.Setup(m => m.Send(createBlogCommand, CancellationToken.None))
                        .ReturnsAsync(expectedResult);

            var controller = new BlogController(mediatorMock.Object);

            
            var actionResult = await controller.CreateBlog(createBlogCommand);

            
            var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
            var blogDto  = okResult.Value.Should().BeOfType<CreateBlogResponseDto>().Subject;

            blogDto.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task CreateBlog_ReturnsBadRequest()
        {
            
            var mediatorMock      = new Mock<IMediator>();
            var createBlogCommand = new CreateBlogCommand(Guid.NewGuid(), "TestTitle", "Test Description", "TestContent");

            mediatorMock.Setup(m => m.Send(createBlogCommand, CancellationToken.None))
                        .ReturnsAsync((CreateBlogResponseDto) null); // Simulate a failure

            var controller = new BlogController(mediatorMock.Object);

            
            var actionResult = await controller.CreateBlog(createBlogCommand);

            
            actionResult.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task GetBlog_ReturnsOkResult()
        {
            
            var mediatorMock   = new Mock<IMediator>();
            var blogId         = Guid.NewGuid();
            var authorInfo     = true;
            var expectedResult = new BlogDto();

            mediatorMock.Setup(m => m.Send(It.IsAny<GetBlogByIdQuery>(), CancellationToken.None))
                        .ReturnsAsync(expectedResult);

            var controller = new BlogController(mediatorMock.Object);

            
            var actionResult = await controller.GetBlog(blogId.ToString(), authorInfo);

            
            var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
            var blogDto  = okResult.Value.Should().BeOfType<BlogDto>().Subject;

            blogDto.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetBlog_ReturnsBadRequest()
        {
            
            var mediatorMock = new Mock<IMediator>();
            var blogId       = Guid.NewGuid();
            var authorInfo   = true;

            mediatorMock.Setup(m => m.Send(It.IsAny<GetBlogByIdQuery>(), CancellationToken.None))
                        .ReturnsAsync((BlogDto) null); // Simulate a failure

            var controller = new BlogController(mediatorMock.Object);

            
            var actionResult = await controller.GetBlog(blogId.ToString(), authorInfo);

            
            actionResult.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task GetBlogList_ReturnsOkResult()
        {
            
            var mediatorMock   = new Mock<IMediator>();
            var authorInfo     = true;
            var expectedResult = new List<BlogDto>();

            mediatorMock.Setup(m => m.Send(It.IsAny<GetBlogListQuery>(), CancellationToken.None))
                        .ReturnsAsync(expectedResult);

            var controller = new BlogController(mediatorMock.Object);

            
            var actionResult = await controller.GetBlogList(authorInfo);

            
            var okResult    = actionResult.Should().BeOfType<OkObjectResult>().Subject;
            var blogListDto = okResult.Value.Should().BeOfType<List<BlogDto>>().Subject;

            blogListDto.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetBlogList_ReturnsBadRequest()
        {
            
            var mediatorMock = new Mock<IMediator>();
            var authorInfo   = true;

            mediatorMock.Setup(m => m.Send(It.IsAny<GetBlogListQuery>(), CancellationToken.None))
                        .ReturnsAsync((List<BlogDto>?) null); // Simulate a failure

            var controller = new BlogController(mediatorMock.Object);

            
            var actionResult = await controller.GetBlogList(authorInfo);

            
            actionResult.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}