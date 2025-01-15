using Microsoft.AspNetCore.Mvc;
using MediatR;
using EventDrivenCQRS.Application.CQRS.Commands.Users;

namespace EventDrivenCQRS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { UserId = result });
        }
    }
}