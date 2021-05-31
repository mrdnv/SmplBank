using MediatR;
using System.Text.Json.Serialization;

namespace SmplBank.Application.Requests
{
    public interface IAuthorizedRequest
    {
        int AccountId { get; set; }
        int UserId { get; set; }
    }

    public abstract class AuthorizedRequest : AuthorizedRequest<Unit>
    {
    }

    public abstract class AuthorizedRequest<TResponse> : BaseRequest<TResponse>, IAuthorizedRequest
    {
        [JsonIgnore]
        public int AccountId { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }
    }

    public abstract class BaseRequest<TResponse> : IRequest<TResponse>
    {

    }
}
