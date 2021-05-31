using MediatR;
using Microsoft.AspNetCore.Authorization;
using SmplBank.Application.Requests;
using SmplBank.Domain.Entity;
using SmplBank.Extensions;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SmplBank.Controllers
{
    [Authorize]
    public abstract class AuthorizedController : BaseController
    {
        protected AuthorizedController(IMediator mediator) : base(mediator)
        {
        }

        private TRequest CreateAuthorizedRequest<TRequest>() where TRequest : IAuthorizedRequest, new()
            => new()
            {
                AccountId = this.HttpContext.GetClaimValue<int>($"{nameof(Account)}{nameof(Account.Id)}"),
                UserId = this.HttpContext.GetClaimValue<int>(ClaimTypes.NameIdentifier)
            };

        protected Task SendAuthorizedAsync<TRequest>(CancellationToken cancellationToken = default) where TRequest : AuthorizedRequest, new()
            => this.SendAsync(CreateAuthorizedRequest<TRequest>(), cancellationToken);

        protected Task<TResponse> SendAuthorizedAsync<TRequest, TResponse>(CancellationToken cancellationToken = default) where TRequest : AuthorizedRequest<TResponse>, new()
            => this.SendAsync(CreateAuthorizedRequest<TRequest>(), cancellationToken);
    }
}
