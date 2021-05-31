using Microsoft.AspNetCore.Mvc.Filters;
using SmplBank.Application.Requests;
using SmplBank.Domain.Entity;
using System;
using System.Linq;
using System.Security.Claims;

namespace SmplBank.Filters
{
    public class AuthorizedRequestTransformActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do nothing.
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var args = context
                .ActionArguments
                .Where(arg => arg.Value is IAuthorizedRequest);

            if (!args.Any())
                return;

            var authorizedRequestParamNAme = args.First().Key;
            var authorizedRequest = args.First().Value as IAuthorizedRequest;

            if (authorizedRequest == null)
            {
                authorizedRequest = Activator.CreateInstance(authorizedRequest.GetType()) as IAuthorizedRequest;
            }

            var claims = context.HttpContext.User?.Identity as ClaimsIdentity;

            if (claims == null)
                return;

            var accountIdClaim = claims.FindFirst($"{nameof(Account)}{nameof(Account.Id)}").Value;

            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out var accountId))
                return;

            authorizedRequest.AccountId = accountId;
        }
    }
}
