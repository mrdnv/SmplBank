using MediatR;
using SmplBank.Application.Requests;
using SmplBank.Domain.Dto.User;
using SmplBank.Domain.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace SmplBank.Application.Handlers
{
    public class InsertUserHandler : IRequestHandler<InsertUseRequest>
    {
        private readonly IUserService userService;

        public InsertUserHandler(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<Unit> Handle(InsertUseRequest request, CancellationToken cancellationToken)
        {
            var dto = new InsertUserDto
            {
                Username = request.Username,
                Password = request.Password
            };

            await this.userService.InsertUserAsync(dto);

            return Unit.Value;
        }
    }
}
