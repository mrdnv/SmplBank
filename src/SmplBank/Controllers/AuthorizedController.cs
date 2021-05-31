using MediatR;
using Microsoft.AspNetCore.Authorization;
using SmplBank.Application.Requests;
using SmplBank.Domain.Entity;
using System.Security.Claims;

namespace SmplBank.Controllers
{
    [Authorize]
    public abstract class AuthorizedController : BaseController
    {
        protected AuthorizedController(IMediator mediator) : base(mediator)
        {
        }

        protected int AccountId => int.Parse(
            (HttpContext.User.Identity as ClaimsIdentity)
            .FindFirst($"{nameof(Account)}{nameof(Account.Id)}").Value);

        protected T CreateAuthorizedRequest<T>() where T : IAuthorizedRequest, new()
        {
            var request = new T
            {
                AccountId = AccountId
            };

            return request;
        }
    }
}
