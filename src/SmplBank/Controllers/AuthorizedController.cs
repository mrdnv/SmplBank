using MediatR;
using Microsoft.AspNetCore.Authorization;
using SmplBank.Application.Requests;
using SmplBank.Domain.Entity;
using SmplBank.Extensions;
using System.Security.Claims;

namespace SmplBank.Controllers
{
    [Authorize]
    public abstract class AuthorizedController : BaseController
    {
        protected AuthorizedController(IMediator mediator) : base(mediator)
        {
        }

        protected T CreateAuthorizedRequest<T>() where T : IAuthorizedRequest, new()
        {
            var request = new T
            {
                AccountId = this.HttpContext.GetClaimValue<int>($"{nameof(Account)}{nameof(Account.Id)}"),
                UserId = this.HttpContext.GetClaimValue<int>(ClaimTypes.NameIdentifier)
            };

            return request;
        }
    }
}
