using Microsoft.AspNetCore.Mvc.Filters;
using SmplBank.Application.Requests;
using SmplBank.Domain.Entity;
using SmplBank.Extensions;
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

            var arg = args.First();
            var authorizedRequest = (arg.Value ?? Activator.CreateInstance(arg.GetType())) as IAuthorizedRequest;
            var claims = context.HttpContext.User?.Identity as ClaimsIdentity;

            if (claims == null)
                return;

            authorizedRequest.AccountId = context.HttpContext.GetClaimValue<int>($"{nameof(Account)}{nameof(Account.Id)}");
            authorizedRequest.UserId = context.HttpContext.GetClaimValue<int>(ClaimTypes.NameIdentifier);
            context.ActionArguments[arg.Key] = authorizedRequest;
        }
    }
}
