using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmplBank.Application.Requests;
using SmplBank.Domain.Entity;
using SmplBank.Extensions;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SmplBank.Controllers
{
    [Authorize]
    public abstract class AuthorizedController : BaseController
    {
        protected AuthorizedController(ISender sender) : base(sender)
        {
        }

        private TRequest CreateAuthorizedRequest<TRequest>() where TRequest : AuthorizedRequest, new()
            => new()
            {
                AccountId = this.HttpContext.GetClaimValue<int>($"{nameof(Account)}{nameof(Account.Id)}"),
                UserId = this.HttpContext.GetClaimValue<int>(ClaimTypes.NameIdentifier)
            };

        private TRequest CreateAuthorizedRequest<TRequest, TResponse>() where TRequest : AuthorizedRequest<TResponse>, new()
            => new()
            {
                AccountId = this.HttpContext.GetClaimValue<int>($"{nameof(Account)}{nameof(Account.Id)}"),
                UserId = this.HttpContext.GetClaimValue<int>(ClaimTypes.NameIdentifier)
            };

        protected Task<IActionResult> SendAuthorizedAsync<TRequest>(CancellationToken cancellationToken = default) where TRequest : AuthorizedRequest, new()
            => this.SendAsync(CreateAuthorizedRequest<TRequest>(), cancellationToken);

        protected Task<IActionResult> SendAuthorizedAsync<TRequest, TResponse>(CancellationToken cancellationToken = default) where TRequest : AuthorizedRequest<TResponse>, new()
            => this.SendAsync(CreateAuthorizedRequest<TRequest, TResponse>(), cancellationToken);

        protected Task<IActionResult> SendAuthorizedAsync<TRequest, TResponse>(Func<TResponse, IActionResult> onSuccess, CancellationToken cancellationToken = default) where TRequest : AuthorizedRequest<TResponse>, new()
            => this.SendAsync(CreateAuthorizedRequest<TRequest, TResponse>(), onSuccess, cancellationToken);
    }
}
