using MediatR;

namespace EventDrivenCQRS.Application.CQRS.Commands.Users
{
    public class CreateUserCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}