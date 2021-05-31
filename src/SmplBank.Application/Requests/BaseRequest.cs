using MediatR;
using System.Text.Json.Serialization;

namespace SmplBank.Application.Requests
{
    public interface IAuthorizedRequest
    {
        int AccountId { get; set; }
        int UserId { get; set; }
    }

    public abstract record AuthorizedRequest : AuthorizedRequest<Unit>
    {
    }

    public abstract record AuthorizedRequest<TResponse> : BaseRequest<TResponse>, IAuthorizedRequest
    {
        [JsonIgnore]
        public int AccountId { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }
    }

    public abstract record BaseRequest<TResponse> : IRequest<TResponse>
    {

    }
}
