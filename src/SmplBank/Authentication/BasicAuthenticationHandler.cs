using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmplBank.Domain.Entity;
using SmplBank.Domain.Exception;
using SmplBank.Domain.Service.Interface;

namespace SmplBank.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserService userService;
        private readonly IAccountService accountService;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock,
            IUserService userService,
            IAccountService accountService) : base(options, logger, encoder, clock)
        {
            this.userService = userService;
            this.accountService = accountService;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authorizationHeader = Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authorizationHeader))
                return AuthenticateResult.Fail("Invalid credential.");

            var authHeader = AuthenticationHeaderValue.Parse(authorizationHeader);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
            var username = credentials[0];
            var password = credentials[1];

            var user = await this.userService.GetByCredentialAsync(username, password);

            if (user == null)
                return AuthenticateResult.Fail("Invalid credential.");

            var account = (await this.accountService.GetByUserIdAsync(user.Id)).SingleOrDefault();

            if (account == null)
                throw new InternalErrorDomainException("User does not associate to any account. Please contact administrator for more information.");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim($"{nameof(Account)}{nameof(Account.Id)}", account.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
