using MediatR;

namespace SmplBank.Application.Requests
{
    public record InsertUseRequest : IRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
