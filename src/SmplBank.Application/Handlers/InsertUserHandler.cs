using SmplBank.Application.Requests.Commands;
using SmplBank.Domain.Dto.User;
using SmplBank.Domain.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace SmplBank.Application.Handlers
{
    public class InsertUserHandler : BaseHandler<InsertUserCommand>
    {
        private readonly IUserService userService;

        public InsertUserHandler(IUserService userService) : base()
        {
            this.userService = userService;
        }

        protected async override Task Process(InsertUserCommand request, CancellationToken cancellationToken)
        {
            var dto = new InsertUserDto
            {
                Username = request.Username,
                Password = request.Password
            };

            await this.userService.InsertUserAsync(dto);
        }
    }
}
