using Microsoft.AspNetCore.Http;
using SmplBank.Domain.Entity;
using System;
using System.Security.Claims;

namespace SmplBank.Extensions
{
    public static class HttpContextExtensions
    {
        public static T GetClaimValue<T>(this HttpContext context, string claimName)
        {
            var claims = context.User?.Identity as ClaimsIdentity;

            if (claims == null)
                return default;

            var claimValue = claims.FindFirst(claimName).Value;

            if (string.IsNullOrEmpty(claimValue) || !int.TryParse(claimValue, out var accountId))
                return default;

            object value = default;

            try
            {
                value = Convert.ChangeType(claimValue, typeof(T));
            }
            catch
            {
            }

            return (T)value;
        }
    }
}
