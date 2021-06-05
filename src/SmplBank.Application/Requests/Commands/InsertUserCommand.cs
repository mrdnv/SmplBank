using MediatR;

namespace SmplBank.Application.Requests.Commands
{
    public record InsertUserCommand : BaseRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
