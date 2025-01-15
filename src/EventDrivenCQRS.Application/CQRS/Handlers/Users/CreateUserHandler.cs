using EventDrivenCQRS.Domain.Entities; // Eklenen satır
using MediatR;
using EventDrivenCQRS.Application.CQRS.Commands.Users;

namespace EventDrivenCQRS.Application.CQRS.Handlers.Users
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email
            };

            // Burada veritabanı işlemi yapılabilir
            return user.Id;
        }
    }
}