using BlogManager.Adapter.Api.Controllers;
using BlogManager.Core.Commands.Author;
using BlogManager.Core.DTOs;
using BlogManager.Core.Queries;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BlogManager.Adapter.Api.Tests
{
    public class AuthorControllerTests
    {
        [Test]
        public async Task CreateAuthor_ReturnsOkResult()
        {
            
            var mediatorMock = new Mock<IMediator>();
            var createAuthorCommand = new CreateAuthorCommand("TestName1", "TestSurname1");
            var expectedResult = new CreateAuthorResponseDto();

            mediatorMock.Setup(m => m.Send(createAuthorCommand, CancellationToken.None))
                        .ReturnsAsync(expectedResult);

            var controller = new AuthorController(mediatorMock.Object);

            
            var actionResult = await controller.CreateAuthor(createAuthorCommand);

            
            var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
            var authorDto = okResult.Value.Should().BeOfType<CreateAuthorResponseDto>().Subject;

            authorDto.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetAuthor_ReturnsOkResult()
        {
            
            var mediatorMock = new Mock<IMediator>();
            var authorId = Guid.NewGuid();
            var expectedResult = new AuthorDto();

            mediatorMock.Setup(m => m.Send(It.IsAny<GetAuthorByIdQuery>(), CancellationToken.None))
                        .ReturnsAsync(expectedResult);

            var controller = new AuthorController(mediatorMock.Object);

            
            var actionResult = await controller.GetAuthor(authorId);

            
            var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
            var authorDto = okResult.Value.Should().BeOfType<AuthorDto>().Subject;

            authorDto.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetAuthor_ReturnsBadRequest()
        {
            
            var mediatorMock = new Mock<IMediator>();
            var authorId = Guid.NewGuid();

            mediatorMock.Setup(m => m.Send(It.IsAny<GetAuthorByIdQuery>(), CancellationToken.None))
                        .ReturnsAsync((AuthorDto)null); // Simulate a failure

            var controller = new AuthorController(mediatorMock.Object);

            
            var actionResult = await controller.GetAuthor(authorId);

            
            actionResult.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task GetAuthorList_ReturnsOkResult()
        {
            
            var mediatorMock = new Mock<IMediator>();
            var expectedResult = new List<AuthorDto>();

            mediatorMock.Setup(m => m.Send(It.IsAny<GetAuthorListQuery>(), CancellationToken.None))
                        .ReturnsAsync(expectedResult);

            var controller = new AuthorController(mediatorMock.Object);

            
            var actionResult = await controller.GetAuthorList();

            
            var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
            var authorListDto = okResult.Value.Should().BeOfType<List<AuthorDto>>().Subject;

            authorListDto.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetAuthorList_ReturnsBadRequest()
        {
            
            var mediatorMock = new Mock<IMediator>();

            mediatorMock.Setup(m => m.Send(It.IsAny<GetAuthorListQuery>(), CancellationToken.None))
                        .ReturnsAsync((List<AuthorDto>?)null); // Simulate a failure

            var controller = new AuthorController(mediatorMock.Object);

            
            var actionResult = await controller.GetAuthorList();

            
            actionResult.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
